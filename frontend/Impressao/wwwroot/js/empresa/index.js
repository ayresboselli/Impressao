function MascaraCNPJ(e) {
    let valor = e.value;
    let tmp = '';

    for (let i = 0; i <= 1; i++) {
        let code = valor.charCodeAt(i)

        if (code >= 48 && code <= 58) {
            tmp += valor.charAt(i);
        }
    }

    if (tmp.length == 2) {
        tmp += '.'
    }

    for (var i = 3; i <= 5; i++) {
        var code = valor.charCodeAt(i)
        if (code >= 48 && code <= 58) {
            tmp += valor.charAt(i);
        }
    }

    if (tmp.length == 6) {
        tmp += '.'
    }

    for (var i = 7; i <= 9; i++) {
        var code = valor.charCodeAt(i)
        if (code >= 48 && code <= 58) {
            tmp += valor.charAt(i);
        }
    }

    if (tmp.length == 10) {
        tmp += '/'
    }

    for (var i = 11; i <= 14; i++) {
        var code = valor.charCodeAt(i)
        if (code >= 48 && code <= 58) {
            tmp += valor.charAt(i);
        }
    }

    if (tmp.length == 15) {
        tmp += '-'
    }

    for (var i = 16; i <= 17; i++) {
        var code = valor.charCodeAt(i)
        if (code >= 48 && code <= 58) {
            tmp += valor.charAt(i);
        }
    }

    e.value = tmp
}

function BuscaCNPJ(cnpj) {
    cnpj = cnpj.replace('.', '').replace('/', '').replace('-', '')

    $.ajax({
        url: 'https://publica.cnpj.ws/cnpj/' + cnpj,
        success: result => {
            if ($('#RazaoSocial').val() == '') { $('#RazaoSocial').val(result.razao_social) }
            if ($('#NomeFantasia').val() == '') { $('#NomeFantasia').val(result.estabelecimento.nome_fantasia) }
            if ($('#DataCriacao').val() == '') { $('#DataCriacao').val(result.estabelecimento.data_inicio_atividade) }
            if ($('#Email').val() == '') { $('#Email').val(result.estabelecimento.email) }

            if ($('#Telefone').val() == '') { $('#Telefone').val('(' + result.estabelecimento.ddd1 + ') ' + result.estabelecimento.telefone1) }
            if (result.estabelecimento.telefone2 != null) {
                if ($('#Telefone2').val() == '') { $('#Telefone2').val('(' + result.estabelecimento.ddd2 + ') ' + result.estabelecimento.telefone2) }
            }

            if ($('#Cep').val() == '') { $('#Cep').val(result.estabelecimento.cep) }
            if ($('#Bairro').val() == '') { $('#Bairro').val(result.estabelecimento.bairro) }
            if ($('#Logradouro').val() == '') { $('#Logradouro').val(result.estabelecimento.logradouro) }
            if ($('#Numero').val() == '') { $('#Numero').val(result.estabelecimento.numero) }
            if ($('#Complemento').val() == '') { $('#Complemento').val(result.estabelecimento.complemento) }
            if ($('#estado').val() == '') { $('#estado').val(result.estabelecimento.estado.sigla) }
            if ($('#CidadeId').val() == '') { $('#CidadeId').val(result.estabelecimento.cidade.ibge_id) }

            Estados()
        }
    })
}

function Estados(uf = '') {
    let id = $('#CidadeId').val()

    if (id != '') {
        $.ajax({
            url: '/cidades.json',
            async: false,
            success: function (cidades) {
                cidades.map((cidade) => {
                    if (cidade.id == parseInt(id)) {
                        uf = cidade.uf
                    }
                });
            },
            error: result => {
                console.log('error', result)
            }
        });
    }

    const estados = ['AC', 'AL', 'AM', 'AP', 'BA', 'CE', 'DF', 'ES',/* 'EX',*/ 'GO', 'MA', 'MG', 'MS', 'MT', 'PA', 'PB', 'PE', 'PI', 'PR', 'RJ', 'RN', 'RO', 'RR', 'RS', 'SC', 'SE', 'SP', 'TO'];
    let html = '<option> -- </option>';

    estados.map((estado) => {
        let selected = '';
        if (estado == uf) {
            selected = ' selected';
        }
        html += "<option" + selected + ">" + estado + "</option>";
    });

    $('#estado').html(html);

    SelecionaEstado(uf);
}

function SelecionaEstado(uf) {
    if (uf != '') {
        $.ajax({
            url: '/cidades.json',
            method: 'get',
            dataType: 'json',
            success: function (cidades) {
                $('#CidadeId').val()
                var html = "<option value=''>-- Selecione --</option>";
                cidades.map((cidade) => {
                    if (cidade.uf == uf) {
                        var selected = '';
                        if ($('#CidadeId').val() == cidade.id) {
                            selected = ' selected';
                        }

                        html += "<option value='" + cidade.id + "'" + selected + ">" + cidade.cidade + "</option>";
                    }
                });

                $('#cidade').html(html);
            }
        });
    }
}

document.addEventListener('DOMContentLoaded', () => {
    Estados()
})