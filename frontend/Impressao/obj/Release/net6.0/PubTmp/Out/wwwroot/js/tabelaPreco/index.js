function Deletar(id) {
    let token = $('input[name="__RequestVerificationToken"]').val();

    Swal.fire({
        title: 'Tem certeza que deseja excluir esta tabela de preços?',
        showCancelButton: true,
        confirmButtonText: 'Deletar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#dc3545'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/TabelaPreco/Delete/' + id,
                method: 'post',
                data: {
                    __RequestVerificationToken: token,
                },
                success: result => {
                    if (result.success) {
                        Swal.fire('Excluído!', '', 'success').then(() => {
                            location.reload()
                        })
                    } else {
                        toastr.error(result.msg)
                    }
                },
                error: function (result) {
                    console.log('Erro', result);
                    toastr.error('Erro ao excluir a tabela de preços');
                }
            });
        }
    })
}

function ModalCliente(id = null, cliente = null, padrao = null) {
    $.ajax({
        url: "/Cliente/Lista",
        method: 'post',
        async: false,
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: result => {

            let html = "<option value=''></option>"
            result.map(r => {
                let selected = ''
                if (cliente == r.id) {
                    selected = " selected='selected'"
                }
                html += "<option value='" + r.id + "'" + selected + ">" + r.razaoSocial + "</option>"
            })
            $('#mCliente').html(html)
        },
        error: result => {
            console.log('erro', result)
        }
    })

    $('#mId').val(id)

    if (padrao == 'True') {
        $('#mPadrao').attr('checked', 'checked')
    } else {
        $('#mPadrao').removeAttr('checked')
    }

    $('#modalCliente').modal('show')
}

function SalvarCliente() {
    let id = $('#mId').val()
    let tabela = $('#Id').val()
    let cliente = $('#mCliente').val()
    let padrao = $('#mPadrao')[0].checked

    if (cliente != '') {
        $.ajax({
            url: "/TabelaPreco/SalvarCliente",
            method: 'post',
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                tabela,
                cliente,
                padrao,
                id
            },
            success: result => {
                console.log(result)
                location.reload()
            },
            error: result => {
                console.log('erro', result)
            }
        })

        $('#modalCliente').modal('hide')
    }
}

function DeletarCliente(id) {
    let token = $('input[name="__RequestVerificationToken"]').val();

    Swal.fire({
        title: 'Tem certeza que deseja desvincular este cliente?',
        showCancelButton: true,
        confirmButtonText: 'Deletar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#dc3545'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/TabelaPreco/DeletarCliente/' + id,
                method: 'post',
                data: {
                    __RequestVerificationToken: token,
                },
                success: result => {
                    if (result.success) {
                        Swal.fire('Desvinculado!', '', 'success').then(() => {
                            location.reload()
                        })
                    } else {
                        toastr.error(result.msg)
                    }
                },
                error: function (result) {
                    console.log('Erro', result);
                    toastr.error('Erro ao desvincular o cliente');
                }
            });
        }
    })
}
