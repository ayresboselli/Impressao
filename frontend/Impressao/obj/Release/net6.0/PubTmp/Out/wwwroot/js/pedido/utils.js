export function GetCliente() {
    let id = $('#cliente').val()

    $.ajax({
        url: '/Pedido/Clientes',
        method: 'get',
        dataType: 'json',
        success: result => {
            let html = '<option value=""></option>';
            result.map(r => {
                let selected = ''
                if (r.id == id) {
                    selected = ' selected'
                }
                html += '<option value="' + r.id + '"' + selected + '>' + r.razaoSocial + '</option>'
            })
            $('#ClienteId').html(html)
        },
        error: result => {
            console.log('error', result)
        }
    })
}

export function GetTabelaPreco() {
    let id = $('#tabela').val()
    let cliente = $('#ClienteId').val()

    $.ajax({
        url: '/Pedido/TabelasPrecos',
        method: 'post',
        dataType: 'json',
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            cliente
        },
        success: result => {
            let html = '';
            result.map(r => {
                let selected = ''
                if (r.id == id) {
                    selected = ' selected'
                }
                html += '<option value="' + r.id + '"' + selected + '>' + r.titulo + '</option>'
            })
            $('#TabelaId').html(html)
        },
        error: result => {
            console.log('error', result)
        }
    })
}

