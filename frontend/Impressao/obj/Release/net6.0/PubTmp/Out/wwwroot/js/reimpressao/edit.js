
function GetItens() {
    let id = $('#Id').val()
    
    $.ajax({
        url: '/PedidoReimpressao/Itens/' + id,
        method: 'get',
        dataType: 'json',
        beforeSend: (jqXHR, settings) => {
            $('#tbReimpressao').html("<tr><td colspan='4' class='text-center'><div class='spinner-border spinner-border-sm'></div></td></tr>")
        },
        success: result => {
            console.log(result)
            if (result.success) {
                let html = '';
                result.arquivos.map(r => {
                    html += "<tr>"
                    html += "    <td>" + r.arquivoFrente.album + "</td>"
                    html += "    <td>" + r.arquivoFrenteId + "</td>"
                    html += "    <td>" + r.arquivoVersoId + "</td>"
                    html += "    <td>"
                    html += "        <a href='javascript:void(0)' onclick='DeleteBlade(" + r.id + ")' title='Excluir' class='text-danger mx-2'><i class='fas fa-trash'></i></a>"
                    html += "    </td>"
                    html += "</tr>"
                })

                if (html == '') {
                    html = "<tr><td colspan='4' class='text-center'>Nenhum item encontrado</td></tr>"
                }

                $('#tbReimpressao').html(html)
            }else{
                Swal.fire('Falhou!', result.msg, 'danger')
            }
        },
        error: result => {
            console.log('error', result)
            Swal.fire('Falhou!', 'Erro ao salvar', 'danger')
        }
    })
}

function Adicionar() {
    let data = {
        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
        PedidoReimpressaoId: $('#Id').val(),
        ArquivoFrenteId: $('#arquivo').val(),
    }

    $.ajax({
        url: '/PedidoReimpressao/Edit',
        method: 'post',
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data,
        success: result => {
            if (result.success) {
                GetItens()
                toastr.success('Lâmina salva com sucesso')
            } else {
                Swal.fire('Falhou!', result.msg, 'error')
            }
        },
        error: result => {
            console.log('error', result)
        }
    })
}

function DeleteBlade(id) {
    Swal.fire({
        title: 'Tem certeza que deseja excluir esta lâmina?',
        showCancelButton: true,
        confirmButtonText: 'Excluir',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#dc3545',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/PedidoReimpressao/DeleteBlade",
                method: "post",
                data: {
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                    id
                },
                success: result => {
                    if (result.success) {
                        Swal.fire('Excluído!', '', 'success')
                        GetItens()
                    } else {
                        Swal.fire('Falhou!', result.msg, 'danger')
                    }
                },
                error: result => {
                    console.log('error', result)
                    Swal.fire('Falhou!', '', 'danger')
                }
            })

        }
    })
}

document.addEventListener('DOMContentLoaded', () => {
    GetItens()
})