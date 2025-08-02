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
    const url = `/Session/DeleteErrorModal`;
    const response = await fetch(url);
    const result = await response.json();

    if (result) {
        const modal = document.getElementById("generalErrorModal");
        modal.classList.add("closeModal");
    }
}

async function closeModalSuccess() {
    const url = `/Session/DeleteSuccessModal`;
    const response = await fetch(url);
    const result = await response.json();

    if (result) {
        const modal = document.getElementById("generalSuccessModal");
        modal.classList.add("closeModal");
    }
}

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

var imageLinks = [];
var sourceLinks = [];

/*
 * Limpia los elementos que son saltos de líneas '\n'
 */
function cleanNodes(element) {
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
let id_aux = 1;

function addList(element, type) {
    //Agregamos elemento al array
    let nodeElementsClean = cleanNodes(element)

    let select = nodeElementsClean[0];
    let input = nodeElementsClean[1];

    //Update o New Media
    let valueSelect = select.value;
    let valueInput = input.value;

    //previousMedia Meta. Si no es prevous, es nuevo, se coloca undefined
    let valueId = input.id || undefined;


    if (valueId == undefined) {
        if (type == "post") {
            imageLinks.push({ id_aux: id_aux, mediatype_id: valueSelect, url: valueInput });
            id_aux++;

        }
        else {
            sourceLinks.push({ id_aux: id_aux, source_id: valueSelect, url: valueInput });
            id_aux++;
        }
    } else {

        if (type == "post") {
            imageLinks.push({ id: valueId, mediatype_id: valueSelect, url: valueInput })
        } else {
            sourceLinks.push({ id: valueId, source_id: valueSelect, url: valueInput })
        }
    }

    //Modificamos la función del botón
    element.classList.remove('btn-primary');
    element.classList.add('btn-secondary');
    element.innerHTML = "Editar";
    element.onclick = function () { editInput(element, type); }

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
function editInput(element, type) {
    let nodeElementsClean = cleanNodes(element)
    let select = nodeElementsClean[0];
    let input = nodeElementsClean[1];
    let btnCancelar = nodeElementsClean[3];

    let valueId = input.id;

    if (type == "post") {
        imageLinks = imageLinks.filter(x => x.id != valueId);
    } else {
        sourceLinks = sourceLinks.filter(x => x.id != valueId);
    }

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
    element.onclick = function () { addList(element, type); }

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
    btnCancelar.onclick = function () { removeInput(element, type); }

    btnEditar.innerHTML = "Editar";
    btnEditar.onclick = function () { editInput(element, type); }
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
function removeInput(element, type) {

    let nodeElementsClean = cleanNodes(element)

    let select = nodeElementsClean[0];
    let input = nodeElementsClean[1];
    let valueId = input.id || undefined;

    if (valueId == undefined) {
        if (type == "post") {
            //Si el link es nuevo, se borra del List
            imageLinks = imageLinks.filter(x => x.mediatype_id != select.value && x.url != input.value);
        } else {
            sourceLinks = sourceLinks.filter(x => x.source_id != select.value && x.url != input.value);
        }
    } else {
        if (type == "post") {
            //Si el link viene desde base de datos, retornamos solamente el id para poder borrarlo en controller
            imageLinks.push({ id: valueId, mediatype_id: undefined, url: undefined });
        } else {
            sourceLinks.push({ id: valueId, source_id: undefined, url: undefined });
        }
    }

    element.parentNode.parentNode.removeChild(element.parentNode);
}

//Agregar categorias
let categoryList = [];

function addCategory(element) {
    const categoryId = element.options[element.selectedIndex].value;
    const categoryName = element.options[element.selectedIndex].text;
    const postId = element.getAttribute("postId");

    //Objeto
    let obj = {
        post_id: parseInt(postId),
        category_id: parseInt(categoryId)
    }

    if (postId == 0) {

        //Post Nuevo
        if (!categoryList.find(c => c.category_id == categoryId)) {
            //El elemento no se encuentra en la lista
            categoryList.push(obj);

            addCategoryTag(categoryName, categoryId)
        }
    } else {
        //Post edicion
        if (!categoryList.find(c => c.category_id == categoryId)) {
            //El elemento no se encuentra en la lista
            categoryList.push(obj);

            addCategoryTag(categoryName, categoryId)
        }
    }


    //Colocamos el selected en la opción inicial
    element.selectedIndex = 0;
}


//Renderizar tag de la categoria en la página
function addCategoryTag(categoryName, categoryId) {
    const categoryContainer = document.getElementById("categoryContainer");

    const categoria = document.createElement("div");
    categoria.classList.add("btn", "tag", "me-2", "mt-3");

    // Agregar el botón para cerrar
    const cerrarCategoria = document.createElement("div");
    cerrarCategoria.classList.add("close-btn"); // Clase CSS para el botón de cierre
    cerrarCategoria.innerText = "x";
    categoria.appendChild(cerrarCategoria);

    // Agregar el icono
    const icon = document.createElement("i");
    icon.classList.add("fas", "fa-tag", "me-1"); // Ajustar las clases según sea necesario
    categoria.appendChild(icon);

    // Agregar el nombre de la categoría
    categoria.appendChild(document.createTextNode(categoryName));
    categoria.onclick = function () { deleteCategory(categoria, categoryId); }

    categoryContainer.appendChild(categoria);
}

//Agregar categoria si hay errores
function addCategoryRefresh(element) {
    const categoryId = element?.id;
    const categoryName = element?.name;
    const postId = element?.postId;

    //Objeto
    let obj = {
        post_id: parseInt(postId),
        category_id: parseInt(categoryId)
    }

    if (!categoryList.find(c => c.category_id == categoryId)) {
        //El elemento no se encuentra en la lista
        categoryList.push(obj);

        addCategoryTag(categoryName, categoryId)
    }

    //Colocamos el selected en la opción inicial
    element.selectedIndex = 0;
}

//Borrar categoria del tag
function deleteCategory(categoria, categoryId) {

    categoryList = categoryList.filter(c => c.category_id != categoryId);

    const categoryContainer = document.getElementById("categoryContainer")
    categoryContainer.removeChild(categoria)

}

// Cargamos los datos del array en el input del formulario
function enviarDatosAlServidor() {
    // Actualiza el valor del campo oculto con los datos del array
    document.getElementById('imageLinksField').value = JSON.stringify(imageLinks);
    document.getElementById('sourceLinksField').value = JSON.stringify(sourceLinks);
    document.getElementById('categoryListField').value = JSON.stringify(categoryList);

    imageLinks = [];
    sourceLinks = [];
    categoryList = [];
}

//Colocamos draft como true
function Borrador() {
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
    let language = document.getElementById("selectLanguage").value;
    text = text.trim();

    switch (format) {
        case "h1":
            return `# ${text}`;
        case "h2":
            return `## ${text}`;
        case "h3":
            return `### ${text}`;
        case "bold":
            return `**${text}**`;
        case "italic":
            return `*${text}*`;
        case "strike":
            return `~~${text}~~`;
        case "inline-code":
            return `\`${text}\``;
        case "link":
            return `[title](${text})`;
        case "image":
            return `![alt text](${text})`;
        case "code":
            return `\`\`\`${language}\n${text}\n\`\`\``;
        case "quote":
            return text.split("\n").map(line => `> ${line.trim()}`).join("\n");
        case "ul":
            return text.split("\n").map(line => `- ${line.trim()}`).join("\n");
        case "ol":
            return text.split("\n").map((line, idx) => `${idx + 1}. ${line.trim()}`).join("\n");
        case "hr":
            return `\n---\n`;
        default:
            return text;
    }
}

/* SELECCIONAR CANTIDAD DE ENTRADAS*/
async function cambiarCantidadEntradas(cantidadEntradas) {
    //Mandar por fetch para guardar session con nueva cantidad por entrada
    const url = `/Session/ChangeNumberPosts/?cantidad=${cantidadEntradas}`
    const response = await fetch(url);
    const result = await response.json();

    if (result) {
        location.reload();
    }
}

async function cambiarCantidadUsers(cantidadUsers) {


    //Mandar por fetch para guardar session con nueva cantidad por entrada
    const url = `/Session/ChangeNumberUsers/?cantidad=${cantidadUsers}`
    const response = await fetch(url);
    const result = await response.json();

    if (result) {

        location.reload();
    }
}

async function buscarUsuarios() {
    let rolerUsers = document.getElementById("roleUserList").value;
    let SearchUsers = document.getElementById("searchUserList").value;

    window.location.href = `/Users/users?role=${rolerUsers}&buscar=${SearchUsers}`;

}

/* BIOS */
async function cargarBio(id) {

    // Fetch
    const url = `/Bio/ObtenerBio/${id}`;
    const response = await fetch(url);
    const result = await response.json();

    // Verificar error
    if (result.error) {
        // Mostrar mensaje de error en el modal correspondiente
        document.getElementById('mensajeModalError').innerText = result.mensaje;
        document.getElementById('modalError').style.display = 'block';
    }

    // Cargar ID
    let inputId = document.getElementById("formIdEditar");
    inputId.value = result.bio.id;

    ////Cargar User
    let user = document.getElementById("formuserEditar");
    user.value = result.bio.user_id;

    // Cargar año
    let year = document.getElementById("formYearEditar");
    let options = Object.entries(year.options);

    options.forEach(x => x[1].value == result.bio.year ? x[1].selected = true : x[1].selected = false)

    // Cargar trabajo
    let work = document.getElementsByClassName("inputFormBioEditar")[0];
    work.value = result.bio.work;
}

/* REDES */

// TODO: Revisar
async function cargarRedes(id) {

    // Fetch
    const urlPost = `/network/ObtenerRedes/${id}`;
    const response = await fetch(urlPost);
    const result = await response.json();


    // Verificar error
    if (result.error) {
        // Mostrar mensaje de error en el modal correspondiente
        document.getElementById('mensajeModalError').innerText = result.mensaje;
        document.getElementById('modalError').style.display = 'block';
    }

    // Cargar ID
    let inputId = document.getElementById("formIdEditar");
    inputId.value = result.bio.id;

    ////Cargar User
    let user = document.getElementById("formUserEditar");
    user.value = result.bio.user_id;

    //Cargar Name
    let name = document.getElementById("formNameEditar");
    name.value = result.bio.name;

    //Cargar URL
    let url = document.getElementById("formUrlEditar");
    url.value = result.bio.url;

    // Cargar username
    let username = document.getElementById("formUsernameEditar");
    username.value = result.bio.username;

    // Cargar Icono
    let icon = document.getElementById("formIconEditar");
    icon.value = result.bio.icon;
}

async function cargarRoles(id) {

    // Fetch
    const urlPost = `/role/ObtenerRol/${id}`;
    const response = await fetch(urlPost);
    const result = await response.json();

    // Verificar error
    if (result.error) {
        // Mostrar mensaje de error en el modal correspondiente
        document.getElementById('mensajeModalError').innerText = result.mensaje;
        document.getElementById('modalError').style.display = 'block';
    }

    // Cargar ID
    let inputId = document.getElementById("formIdRole");
    inputId.value = result.role.id;

    // Cargar nombre del rol
    let name = document.getElementById("inputFormRoleEditar");
    name.value = result.role.name;
}


/* BARRAS NAVEGACION */
function handleMouseOver(event) {

    event.currentTarget.children[1].classList.add("visible");
    event.currentTarget.children[1].classList.remove("invisible");
    event.currentTarget.children[1].classList.add("item_size");
}

function handleMouseOut(event) {
    //var collapse = event.currentTarget.href.split("/").filter(x => x.includes("#"))[0];

    event.currentTarget.children[1].classList.add("invisible");
    event.currentTarget.children[1].classList.remove("visible");
    event.currentTarget.children[1].classList.remove("item_size");
    //new bootstrap.Collapse(collapse, { toggle: false })
}

function actionBarCollapse() {
    /** TITULOS PRINCIPAL DE BARRA LATERAL**/
    let navTitle = document.getElementById("navTitle");
    navTitle.classList.add("nav-title-page-dimension");
    navTitle.classList.add("nav-title-page-collapse");
    navTitle.classList.remove("nav-title-page");

    /** PROFILE **/
    let profileName = document.getElementById("profileName");
    profileName.classList.remove("profile-name-nav");
    profileName.classList.add("profile-name-nav-collapse");

    /** ITEMS MAIN **/
    let navItem = document.querySelectorAll("#nav_items");
    navItem.forEach(x => {
        x.classList.add("collapse-relative");
    });

    /** ITEMS LI **/
    let itemLi = document.querySelectorAll("#itemLi");
    itemLi.forEach((x, i) => {
        x.classList.add("nav-title-page-collapse");
        x.classList.remove("nav-title");
        x.classList.add("nav-link-collapse");
    });

    /** ITEMS TITULOS COLLAPSE **/
    let subNavTitleCollapse = document.querySelectorAll("#sub-nav-title-collapse");
    subNavTitleCollapse.forEach(x => x.classList.remove("hide"))

    /** CONTENIDO LATERAL **/
    let lateralBarContent = document.getElementById("lateral-bar-content");
    lateralBarContent.classList.add("content-collapse")
    lateralBarContent.classList.remove("content")

    /** ITEMS TITULOS EXPANDIDOS **/
    let itemTitle = document.querySelectorAll("#itemTitle");
    itemTitle.forEach(x => x.classList.add("hide"));

    /** ICONOS CIRCULOS **/
    let subIcons = document.querySelectorAll(".subiconos");
    subIcons.forEach(x => x.classList.add("hide"));

    /** FONDO DE NAVEGACION COLLAPSE**/
    let itemSubNav = document.querySelectorAll(".item_subNav");
    itemSubNav.forEach(x => x.classList.add("item_subNav_bg"));

    /** POSICION ABSOLUTA DE ITEMS**/
    let subMenuPost = document.getElementById("subMenuPosts");
    subMenuPost.classList.add("collapse-absolute");

    let subMenuCategories = document.getElementById("subMenuCategories");
    subMenuCategories.classList.add("collapse-absolute");

    let subMenuFormats = document.getElementById("subMenuFormats");
    subMenuFormats.classList.add("collapse-absolute");

    let subMenuMediaTypes = document.getElementById("subMenuMediaTypes");
    subMenuMediaTypes.classList.add("collapse-absolute");

    let subMenuSources = document.getElementById("subMenuSources");
    subMenuSources.classList.add("collapse-absolute");

    let subMenuMediaUsers = document.getElementById("subMenuUsers");
    subMenuMediaUsers.classList.add("collapse-absolute");

    let subMenuMediaRol = document.getElementById("subMenuRole");
    subMenuMediaRol.classList.add("collapse-absolute");
}

function actionBarExpand() {
    /** TITULOS PRINCIPAL DE BARRA LATERAL**/
    let navTitle = document.getElementById("navTitle");
    navTitle.classList.remove("nav-title-page-dimension");
    navTitle.classList.remove("nav-title-page-collapse");
    navTitle.classList.add("nav-title-page");

    /** PROFILE **/
    let profileName = document.getElementById("profileName");
    profileName.classList.add("profile-name-nav");
    profileName.classList.remove("profile-name-nav-collapse");

    /** ITEMS MAIN **/
    let navItem = document.querySelectorAll("#nav_items");
    navItem.forEach(x => {
        x.classList.remove("collapse-relative");
    });

    /** ITEMS LI **/
    let itemLi = document.querySelectorAll("#itemLi");
    itemLi.forEach((x, i) => {
        x.classList.remove("nav-title-page-collapse");
        x.classList.add("nav-title");
        x.classList.remove("nav-link-collapse");
    });

    /** ITEMS TITULOS COLLAPSE **/
    let subNavTitleCollapse = document.querySelectorAll("#sub-nav-title-collapse");
    subNavTitleCollapse.forEach(x => x.classList.add("hide"))

    /** ITEMS TITULOS EXPANDIDOS **/
    let itemTitle = document.querySelectorAll("#itemTitle");
    itemTitle.forEach(x => x.classList.remove("hide"));

    /** CONTENIDO LATERAL **/
    let lateralBarContent = document.getElementById("lateral-bar-content");
    lateralBarContent.classList.remove("content-collapse")
    lateralBarContent.classList.add("content")

    /** ICONOS CIRCULOS **/
    let subIcons = document.querySelectorAll(".subiconos");
    subIcons.forEach(x => x.classList.remove("hide"));

    /** FONDO DE NAVEGACION COLLAPSE**/
    let itemSubNav = document.querySelectorAll(".item_subNav");
    itemSubNav.forEach(x => x.classList.remove("item_subNav_bg"));

    /** POSICION ABSOLUTA DE ITEMS**/
    let subMenuPost = document.getElementById("subMenuPosts");
    subMenuPost.classList.remove("collapse-absolute");

    let subMenuCategories = document.getElementById("subMenuCategories");
    subMenuCategories.classList.remove("collapse-absolute");

    let subMenuFormats = document.getElementById("subMenuFormats");
    subMenuFormats.classList.remove("collapse-absolute");

    let subMenuMediaTypes = document.getElementById("subMenuMediaTypes");
    subMenuMediaTypes.classList.remove("collapse-absolute");

    let subMenuSources = document.getElementById("subMenuSources");
    subMenuSources.classList.remove("collapse-absolute");

    let subMenuMediaUsers = document.getElementById("subMenuUsers");
    subMenuMediaUsers.classList.remove("collapse-absolute");

    let subMenuMediaRol = document.getElementById("subMenuRole");
    subMenuMediaRol.classList.remove("collapse-absolute");
}

function actionBar(mode) {
    if (mode) {
        actionBarCollapse();
    } else {
        actionBarExpand();
    }
}

/* TOAST */

function openToast() {
    var x = document.getElementById("snackbar");
    x.className = "show";
    setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
}

/* HANDLE LINKS */

function handleLinkClickCloseModal(event, url) {
    event.preventDefault();

    closeModalError();

    window.location.href = url;
}

/* MENU */

function actionBarHide(obj)
{
    let childs = Array.from(obj.childNodes);
    let mode = obj.getAttribute("collapse") == "true";
    let ulElement = childs.find(el => el.localName == "ul")

    if (mode) {
        ulElement.classList.remove("collapse")
        ulElement.classList.remove("hide")
        ulElement.classList.remove("transitionSmooth")

    }
    else {
        ulElement.classList.add("collapse")
        ulElement.classList.add("hide")
        ulElement.classList.add("transitionSmooth")
    }

    obj.setAttribute("collapse", !mode)

}