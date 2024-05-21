function ValidaId(id) {
    $.ajax({
        url: '/Usuario/ValidaId/' + id,
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

function UsuarioPerfis() {
    let perfis = $('.usuarioPerfis')
    let ids = []

    for (let i = 0; i < perfis.length; i++) {
        if (perfis[i].checked) {
            ids.push(perfis[i].value)
        }
    }

    $('#usuarioPerfis').val(ids.join(','))
}

document.addEventListener('DOMContentLoaded', () => {
    if ($('#Id').val() == 0 || $('#Id').val() == '') {
        $('#Id').val('')
        $('#Id').removeAttr('readonly')
    }

    UsuarioPerfis()
})