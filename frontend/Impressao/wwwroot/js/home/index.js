function PedidosAtivos() {
    $.ajax({
        url: '/Home/PedidosAtivos',
        method: 'post',
        dataType: 'json',
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
        },
        success: result => {
            $('#cardPedidosAtivos').text(result.pedidosAtivos)
        },
        error: result => {
            console.log('error', result)
        }
    })
}

function PedidosFinalizados() {
    $.ajax({
        url: '/Home/PedidosFinalizados',
        method: 'post',
        dataType: 'json',
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
        },
        success: result => {
            $('#cardPedidosFinalizados').text(result.pedidosFinalizados)
        },
        error: result => {
            console.log('error', result)
        }
    })
}
/*
function PedidosMes() {
    $.ajax({
        url: '/Home/PedidosMes',
        method: 'post',
        dataType: 'json',
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
        },
        success: result => {
            console.log(result)
        },
        error: result => {
            console.log('error', result)
        }
    })
}

function AlbunsMes() { }
function FotosMes() { }
function ProdutosTop5Pedidos() { }
function ProdutosTop5Fotos() { }
*/
document.addEventListener('DOMContentLoaded', () => {
    PedidosAtivos()
    PedidosFinalizados()
    
})