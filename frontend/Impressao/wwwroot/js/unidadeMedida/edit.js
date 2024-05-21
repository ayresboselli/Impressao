function ValidaId(id) {
    $.ajax({
        url: '/UnidadeMedida/ValidaId/' + id,
        method: 'get',
        dataType: 'json',
        success: result => {
            if (result.valid) {
                $('#Id').removeClass('is-invalid')
                $('#Id').addClass('is-valid')
            } else {
                $('#Id').removeClass('is-valid')
                $('#Id').addClass('is-invalid')
                $('#Id').val('')
            }
        },
        error: result => {
            console.log('error', result)
        }
    })
}

document.addEventListener('DOMContentLoaded', () => {
    if ($('#Id').val() == 0 || $('#Id').val() == '') {
        $('#Id').val('')
        $('#Id').removeAttr('readonly')
    }
})