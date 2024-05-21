
function Delete(id) {
    Swal.fire({
        title: 'Tem certeza que deseja deletar este pedido?',
        showCancelButton: true,
        confirmButtonText: 'Deletar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#dc3545',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: "/PedidoReimpressao/Delete",
                method: "post",
                data: {
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                    id
                },
                success: result => {
                    if (result.success) {
                        Swal.fire('Deletado!', '', 'success').then((result) => {
                            location.reload()
                        })
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
