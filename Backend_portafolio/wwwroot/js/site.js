// MODAL
function openModal(obj) {
	const modal = document.getElementById("modal");
	const inputModal = document.getElementsByName("id")[0];
	modal.classList.remove("closeModal")
	inputModal.value = obj.id;
	const modalMsg = document.getElementById("modalMsg");
	modalMsg.innerHTML = `¿Está seguro que quiere borrar ${obj.name}?`;
}

function closeModal() {
	const modal = document.getElementById("modal");
	modal.classList.add("closeModal");
}

$('.btn-expand-collapse').click(function (e) {
	$('.navbar-primary').toggleClass('collapsed');
});

//ADD MEDIA

var imageLinks = [];

/*
 * Limpia los elementos que son saltos de líneas '\n'
 */
function cleanNodes(element)
{
    let nodeElements = element.parentNode.childNodes;
    let nodeElementsClean = [];

    for (i = 0; i < nodeElements.length; i++) {

        if (nodeElements[i]?.nodeValue === null) {
            nodeElementsClean.push(nodeElements[i]);
        }

    }

    return nodeElementsClean;
}

function addList(element) {
    //Agregamos elemento al array
    let nodeElementsClean = cleanNodes(element)

    let select = nodeElementsClean[0];
    let input = nodeElementsClean[1];

    //Update o New Media
    let valueSelect = select.value;
    let valueInput = input.value;

    //previousMedia Meta. Si no es prevous, es nuevo, se coloca undefined
    let valueId = input.id|| undefined;

    if (valueId == undefined)
        imageLinks.push({ mediatype_id: valueSelect, url: valueInput });
    else
        imageLinks.push({ id: valueId, mediatype_id: valueSelect, url: valueInput })

    //Modificamos la función del botón
    element.classList.remove('btn-primary');
    element.classList.add('btn-secondary');
    element.innerHTML = "Editar";
    element.onclick = function () { editInput(element); }

    //Deshabilitamos los cambios en el input
    input.disabled = true;
    input.classList.remove('input-style');
    input.classList.add('input-style-disabled');

    select.disabled = true;
    select.classList.remove('input-style');
    select.classList.add('input-style-disabled');
}
function editInput(element) {

    let nodeElementsClean = cleanNodes(element)

    let select = nodeElementsClean[0];
    let input = nodeElementsClean[1];
    let valueSelect = select.value;
    let valueInput = input.value;

    //Eliminamos elemento
    imageLinks = imageLinks.filter(x => x.valueSelect != valueSelect && x.valueInput != valueInput);

    //Habilitamos input para edición
    input.disabled = false;
    input.classList.remove('input-style-disabled');
    input.classList.add('input-style');

    //Habilitamos el <select>
    select.disabled = false;
    select.classList.remove('input-style-disabled');
    select.classList.add('input-style');

    //Modificamos boton Editar a Agregar
    element.classList.remove('btn-secondary');
    element.classList.add('btn-primary');
    element.innerHTML = "Agregar";

    //Agregamos función de Agregar
    element.onclick = function () { addList(element); }

}

function removeInput(element) {

    let nodeElementsClean = cleanNodes(element)

    let select = nodeElementsClean[0];
    let input = nodeElementsClean[1];
    let valueId = input.id || undefined;

    if (valueId == undefined) {
        //Si el link es nuevo, se borra del List
        imageLinks = imageLinks.filter(x => x.mediatype_id != select.value && x.url != input.value);
    } else {
        //Si el link viene desde base de datos, retornamos solamente el id para poder borrarlo en controller
        imageLinks.push({ id: valueId, mediatype_id: undefined, url: undefined});
    }

    element.parentNode.parentNode.removeChild(element.parentNode);
}

function enviarDatosAlServidor() {
    // Actualiza el valor del campo oculto con los datos del array
    document.getElementById('imageLinksField').value = JSON.stringify(imageLinks);
    imageLinks = [];
}