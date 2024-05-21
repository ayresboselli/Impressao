function ValidaId(id) {
    $.ajax({
        url: '/Perfil/ValidaId/' + id,
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

function PerfilFuncoes() {
    let funcoes = $('.perfilFuncoes')
    let ids = []

    for (let i = 0; i < funcoes.length; i++) {
        if (funcoes[i].checked) {
            ids.push(funcoes[i].value)
        }
    }

    $('#perfilFuncoes').val(ids.join(','))
}

document.addEventListener('DOMContentLoaded', () => {
    if ($('#Id').val() == 0 || $('#Id').val() == '') {
        $('#Id').val('')
        $('#Id').removeAttr('readonly')
    }

    PerfilFuncoes()
})