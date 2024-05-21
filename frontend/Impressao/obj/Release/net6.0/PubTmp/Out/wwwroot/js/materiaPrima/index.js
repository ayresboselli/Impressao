function Deletar(id) {
    let token = $('input[name="__RequestVerificationToken"]').val();

    Swal.fire({
        title: 'Tem certeza que deseja excluir esta matéria prima?',
        showCancelButton: true,
        confirmButtonText: 'Deletar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#dc3545'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/MateriaPrima/Delete/' + id,
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
                    toastr.error('Erro ao excluir a matéria prima');
                }
            });
        }
    })
}
