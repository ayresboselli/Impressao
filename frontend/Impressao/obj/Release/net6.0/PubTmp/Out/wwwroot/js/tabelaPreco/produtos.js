function ListarProdutos() {
    $.ajax({
        url: "/TabelaPreco/ListaProdutos",
        method: 'post',
        async: false,
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            tabela: $('#Id').val()
        },
        success: result => {
            let html = ""
            result.map(r => {
                let id = r.id ? r.id : null
                let valor = r.valor ? r.valor.toFixed(2).replace('.',',') : ''
                html += "<tr>"
                html += "    <td>" + r.produtoId + "</td>"
                html += "    <td>" + r.produto + "</td>"
                html += "    <td><input type=text class='form-control form-control-sm decimal' value='" + valor + "' onchange='SalvarProduto(this, " + r.produtoId + ", " + id + ")'></td>"
                html += "</tr>"
            })
            $('#tbProdutos').html(html)
        },
        error: result => {
            console.log('erro', result)
        }
    })
}

function SalvarProduto(elem, produto, id = null) {
    let tabela = $('#Id').val()
    //valor = parseFloat(elem.value.replace(',', '.'))
    valor = elem.value

    if (produto != '') {
        $.ajax({
            url: "/TabelaPreco/SalvarProduto",
            method: 'post',
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                tabela,
                produto,
                valor,
                id
            },
            success: result => {
                console.log(result)
                if (result.success) {
                    elem.className += ' is-valid'
                } else {
                    elem.className += ' is-invalid'
                }
            },
            error: result => {
                console.log('erro', result)
            }
        })

        $('#modalProduto').modal('hide')
    }
}

document.addEventListener('DOMContentLoaded', () => {
    ListarProdutos()
})