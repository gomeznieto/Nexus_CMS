// MODAL
function openModal(categoria) {
	const modal = document.getElementById("modal");
	const inputModal = document.getElementsByName("id")[0];
	modal.classList.remove("closeModal")
	inputModal.value = categoria.id;
	const modalMsg = document.getElementById("modalMsg");
	modalMsg.innerHTML = `¿Está seguro que quiere borrar la categoria ${categoria.name}?`;
}

function closeModal() {
	const modal = document.getElementById("modal");
	modal.classList.add("closeModal");
}

