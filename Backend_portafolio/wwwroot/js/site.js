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

