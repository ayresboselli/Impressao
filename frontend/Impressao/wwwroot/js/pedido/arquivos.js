function ListaAlbuns() {
	$.ajax({
		url: "/Pedido/ListaAlbuns",
		method: "post",
		async: false,
		data: {
			__RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
			id: $('#Id').val(),
		},
		success: result => {
			if (result.success) {
				let first = false
				$('#album').append("")
				result.albuns.map(a => {
					$('#album').append("<option>" + a + "</option>")

					if (!first) {
						ListaArquivos(a)
						first = true
					}
				})
			}
		},
		error: result => {
			console.log('error',result)
		}
	})
}

function ListaArquivos(album) {
	$.ajax({
		url: "/Pedido/ListaArquivos",
		method: "post",
		async: false,
		data: {
			__RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
			id: $('#Id').val(),
			album
		},
		success: result => {
			if (result.success) {
				$('#listaFotosSimplex').html("");
				$('#listaFotosDuplex').html("")

				let cnt = 0
				result.arquivos.map(a => {
					lista_arquivos.push(a)
					if ($('#Produto_DisposicaoImagem').val() == 'D') {
						// Duplex
						$('#livro').show()
						$('#listaFotosDuplex').show()
						$('#listaFotosDuplex').append("<a href='javascript:void(0)' onclick='Livro(" + cnt + ")'><img src='/img/thumbs/" + a.path + "'></a>")
					} else {
						// Simplex
						$('#livro').hide()
						$('#listaFotosDuplex').hide()
						$('#listaFotosSimplex').append("<a href='javascript:void(0)' onclick=\"VerArquivo('/img/thumbs/" + a.path + "')\"><img src='/img/thumbs/" + a.path + "'></a>")
					}
					cnt++
				})
			}
		},
		error: result => {
			console.log('error', result)
		}
	})
}

function Livro(index) {
	var urlPar = '';
	var urlImpar = '';
	var preview = 0;
	var next = 0;

	if (index % 2 == 0) {
		if (index > 0) {
			urlImpar = '/img/thumbs/' + lista_arquivos[index - 1].path;
			preview = index - 2;
		}

		urlPar = '/img/thumbs/' + lista_arquivos[index].path;

		if (index < lista_arquivos.length - 1) {
			next = index + 1;
		}
	} else {
		if (index > 0) {
			preview = index - 1;
		}

		urlImpar = '/img/thumbs/' + lista_arquivos[index].path;

		if (index < lista_arquivos.length - 1) {
			urlPar = '/img/thumbs/' + lista_arquivos[index + 1].path;
			next = index + 2;
		}
	}

	$('#imgPagPar').attr('src', urlPar);
	$('#imgPagImpar').attr('src', urlImpar);

	$('#aImgPagImpar').attr('onclick', 'Livro(' + preview + ')');
	$('#aImgPagPar').attr('onclick', 'Livro(' + next + ')');
}

function VerArquivo(url) {
	$('#fotoModal').attr('src', '');
	$('#fotoModal').attr('src', url);
	$('#modalVerArquivo').modal('show');
}

let lista_arquivos = []
document.addEventListener('DOMContentLoaded', () => {
	
	ListaAlbuns()
	Livro(0)
});