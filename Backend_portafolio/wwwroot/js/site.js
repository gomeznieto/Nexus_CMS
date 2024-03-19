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

async function closeModalError() {
    const url = `/Modals/DeleteErrorModal`;
    const response = await fetch(url);
    const result = await response.json();

    if (result) {
        const modal = document.getElementById("generalErrorModal");
        modal.classList.add("closeModal");
    }


}

//MENU COLLAPSE
$('.btn-expand-collapse').click(function (e) {
	$('.navbar-primary').toggleClass('collapsed');
});

//MODEL RESPUESTA
async function verificarRespuestaModal(urlPath) {
    const id = document.getElementsByName("id")[0].value;
    const url = urlPath + id;

    //Borrar
    const response = await fetch(url, {
        method: 'POST'
    });

    const result = await response.json();

    if (result.error) {
        // Mostrar mensaje de error en el modal correspondiente
        document.getElementById('mensajeModalError').innerText = result.mensaje;
        document.getElementById('modalError').style.display = 'block';

    } else {
        // Mostrar mensaje de éxito en el modal correspondiente
        document.getElementById('mensajeModalSuccess').innerText = result.mensaje;
        document.getElementById('modalSuccess').style.display = 'block';
    }

    // Cerrar modal de borrado
    closeModal();
}

function closeModalResult(modalId) {
    document.getElementById(modalId).style.display = 'none';
    location.reload();
}

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

/*
 * Agregamos select e inputl al array
 */
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

/*
 * habilitamos select e input para poder editar. Si aceptamos agregamos al array.
 */
function editInput(element) {

    let nodeElementsClean = cleanNodes(element)

    let select = nodeElementsClean[0];
    let input = nodeElementsClean[1];
    let btnCancelar = nodeElementsClean[3];

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

    //Modificamos el botón de eliminar
    btnCancelar.innerHTML = "Cancelar";
    btnCancelar.onclick = function () { cancelInput(element); }


}

/*
 * Regresamos botones previos a Editar
 */
function cancelInput(element) {
    let nodeElementsClean = cleanNodes(element)
    let btnCancelar = nodeElementsClean[3];
    let btnEditar = nodeElementsClean[2];
    let select = nodeElementsClean[0];
    let input = nodeElementsClean[1];

    btnCancelar.innerHTML = "Eliminar";
    btnCancelar.onclick = function () { removeInput(element); }

    btnEditar.innerHTML = "Editar";
    btnEditar.onclick = function () { editInput(element); }
    btnEditar.classList.remove("btn-primary")
    btnEditar.classList.add("btn-secondary")

    //Habilitamos input para edición
    input.disabled = true;
    input.classList.remove('input-style');
    input.classList.add('input-style-disabled');

    //Habilitamos el <select>
    select.disabled = true;
    select.classList.remove('input-style');
    select.classList.add('input-style-disabled');
}

/*
 * LiRemovemos items del array si el input es nuevo, sino se agrega el id y valores en default para borrar en controller
 */
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

// Cargamos los datos del array en el input del formulario
function enviarDatosAlServidor() {
    console.log("EnviarDatosAlServidor")
    // Actualiza el valor del campo oculto con los datos del array
    document.getElementById('imageLinksField').value = JSON.stringify(imageLinks);
    imageLinks = [];
}

//Colocamos draft como true
function Borrador() {
    console.log("Borrador")
    const inputDraft = document.getElementById("inputDraft");
    inputDraft.value = true;
}

/* BUSCAR */
function buscarPost(element) {

    let nodeElementsClean = cleanNodes(element.parentNode)

    const inputValue = nodeElementsClean[0].value;
    window.location.href = `Posts?format=Project&buscar=${inputValue}`;
}

/* BORRADOR */

async function borrador() {
    let inputDraft = document.getElementById("draftCheckBox");
    let postId = document.getElementById("post_id").value;
    let inputDraftValue = inputDraft.value == "True";
    inputDraft.value = inputDraftValue ? "False" : "True";

    let btnBorrador = document.getElementById("btnBorrador");
    btnBorrador.innerHTML = inputDraftValue ? "Borrador" : "Publicar"

    //Fetch
    let url = `/Posts/EditarBorrador/?id=${postId}&draft=${!inputDraftValue}`
    const response = await fetch(url, {
        method: 'POST'
    });

    const result = await response.json();

    if (result.error) {
        // Mostrar mensaje de error en el modal correspondiente
        document.getElementById('mensajeModalError').innerText = result.mensaje;
        document.getElementById('modalError').style.display = 'block';

    } else {
        // Mostrar mensaje de éxito en el modal correspondiente
        document.getElementById('mensajeModalSuccess').innerText = result.mensaje;
        document.getElementById('modalSuccess').style.display = 'block';
    }

}

/* POST */

function addTextFormat(style) {
    let txtarea = document.getElementById("post_text");
    const startPos = txtarea.selectionStart;
    const endPos = txtarea.selectionEnd;
    const before = txtarea.value.substring(0, startPos);
    const after = txtarea.value.substring(startPos, txtarea.value.length);
    var selectedText = txtarea.value.substring(startPos, endPos);
    let styleFormat = textFormat(style, selectedText);

    txtarea.value = before + `${styleFormat} ` + after.substring(selectedText.length);
    txtarea.selectionStart = txtarea.selectionEnd = startPos + styleFormat.length;
    txtarea.focus(); 
}

function textFormat(format, text) {

    switch (format) {
        case "h1":
            return `# ${text}`;
            break;
        case "h2":
            return `## ${text}`;
            break;
        case "h3":
            return `### ${text}`;
            break;
        case "bold":
            return `**${text}**`;
            break;
        case "italic":
            return `*${text}*`;
            break;
        case "link":
            return `[title](${text})`;
            break;
        case "image":
            return `![alt text](${text})`;
            break;
        default:
            return "?";
            break;
    }
}

/* SELECCIONAR CANTIDAD DE ENTRADAS*/
async function cambiarCantidadEntradas(cantidadEntradas) {
    //Mandar por fetch para guardar session con nueva cantidad por entrada
    const url = `/Modals/ChangeNumberPosts/?cantidad=${cantidadEntradas}` 
    const response = await fetch(url);
    const result = await response.json();

    if (result) {
        location.reload();
    }
}