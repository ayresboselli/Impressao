function ValidaId(id) {
    $.ajax({
        url: '/MateriaPrima/ValidaId/' + id,
        method: 'get',
        dataType: 'json',
        success: result => {
            if (result.valid) {
                $('#Materia_Id').removeClass('is-invalid')
                $('#Materia_Id').addClass('is-valid')
            } else {
                $('#Materia_Id').removeClass('is-valid')
                $('#Materia_Id').addClass('is-invalid')
                $('#Materia_Id').val('')
            }
        },
        error: result => {
            console.log('error', result)
        }
    })
}

document.addEventListener('DOMContentLoaded', () => {
    if ($('#Materia_Id').val() == 0 || $('#Id').val() == '') {
        $('#Materia_Id').val('')
        $('#Materia_Id').removeAttr('readonly')
    }
})