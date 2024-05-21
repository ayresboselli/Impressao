function ValidaId(id) {
    $.ajax({
        url: '/UnidadeMedidaConversao/ValidaId/' + id,
        method: 'get',
        dataType: 'json',
        success: result => {
            if (result.valid) {
                $('#Conversao_Id').removeClass('is-invalid')
                $('#Conversao_Id').addClass('is-valid')
            } else {
                $('#Conversao_Id').removeClass('is-valid')
                $('#Conversao_Id').addClass('is-invalid')
                $('#Conversao_Id').val('')
            }
        },
        error: result => {
            console.log('error', result)
        }
    })
}

document.addEventListener('DOMContentLoaded', () => {
    if ($('#Conversao_Id').val() == 0 || $('#Conversao_Id').val() == '') {
        $('#Conversao_Id').val('')
        $('#Conversao_Id').removeAttr('readonly')
    }
})