function ValidaId(id) {
    $.ajax({
        url: '/Cliente/ValidaId/' + id,
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

function MascaraCPF(e) {
    var x = e.value.replace(/\D/g, '').match(/(\d{0,3})(\d{0,3})(\d{0,3})(\d{0,2})/);
    e.value = !x[2] ? x[1] : x[1] + '.' + x[2] + '.' + x[3] + (x[4] ? '-' + x[4] : '');
}

function MascaraCNPJ(e) {
    var x = e.value.replace(/\D/g, '').match(/(\d{0,2})(\d{0,3})(\d{0,3})(\d{0,4})(\d{0,2})/);
    e.value = !x[2] ? x[1] : x[1] + '.' + x[2] + '.' + x[3] + '/' + x[4] + (x[5] ? '-' + x[5] : '');

    if (e.value.length == 18) {
        BuscaCNPJ(e.value)
    }
}

function MascaraCEP(e) {
    var x = e.value.replace(/\D/g, '').match(/(\d{0,5})(\d{0,3})/);
    e.value = !x[2] ? x[1] : x[1] + '-' + x[2];
}

function MascaraTelefone(e) {
    var x = e.value.replace(/\D/g, '').match(/(\d{0,2})(\d{0,9})/);
    e.value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2];
}

function BuscaCNPJ(cnpj) {
    cnpj = cnpj.replace('.', '').replace('/', '').replace('-', '')

    $.ajax({
        url: 'https://publica.cnpj.ws/cnpj/' + cnpj,
        success: result => {
            if ($('#RazaoSocial').val() == '') { $('#RazaoSocial').val(result.razao_social) }
            if ($('#NomeFantasia').val() == '') { $('#NomeFantasia').val(result.estabelecimento.nome_fantasia) }
            if ($('#DataNascimento').val() == '') { $('#DataNascimento').val(result.estabelecimento.data_inicio_atividade) }
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

function BuscaCEP(cep) {
    cep = cep.replace('-', '')
    
    $.ajax({
        url: 'https://viacep.com.br/ws/' + cep + '/json/',
        success: result => {
            if ($('#Bairro').val() == '') { $('#Bairro').val(result.bairro) }
            if ($('#Logradouro').val() == '') { $('#Logradouro').val(result.logradouro) }
            if ($('#estado').val() == '') { $('#estado').val(result.uf) }
            if ($('#CidadeId').val() == '') { $('#CidadeId').val(result.ibge) }

            Estados()
        }
    })
}

function TipoPessoa() {
    var fj = $('#FisicaJuridica').val();

    if (fj == 'J') {
        $('#lb_razao_social').html("<span class='text-danger'>* </span>Razão social");
        $('#lb_nome_fantasia').html('Nome fantasia');
        $('#lb_cpf_cnpj').html("<span class='text-danger'>* </span>CNPJ");
        $('#CpfCnpj').attr('placeholder', '00.000.000/0000-00');
        $('#CpfCnpj').attr('onkeyup', 'MascaraCNPJ(this)');
        $('#CpfCnpj').attr('onblur', 'MascaraCNPJ(this)');
        $('#lb_data_nasc').html("<span class='text-danger'>* </span>Data de cadastro");
    } else {
        $('#lb_razao_social').html("<span class='text-danger'>* </span>Nome completo");
        $('#lb_nome_fantasia').html('Apelido');
        $('#lb_cpf_cnpj').html("<span class='text-danger'>* </span>CPF");
        $('#CpfCnpj').attr('placeholder', '000.000.000-00');
        $('#CpfCnpj').attr('onkeyup', 'MascaraCPF(this)');
        $('#CpfCnpj').attr('onblur', 'MascaraCPF(this)');
        $('#CpfCnpj').removeAttr('onblur');
        $('#lb_data_nasc').html("<span class='text-danger'>* </span>Data de nascimento");
    }
}

function GetGrupo() {
    let id = $('#grupo').val()

    $.ajax({
        url: '/Cliente/Grupos',
        method: 'get',
        dataType: 'json',
        success: result => {
            let html = '<option value=""></option>';
            result.map(r => {
                let selected = ''
                if (r.id == id) {
                    selected = ' selected'
                }
                html += '<option value="'+r.id+'"'+selected+'>'+r.titulo+'</option>'
            })
            $('#ClienteGrupoId').html(html)
        },
        error: result => {
            console.log('error',result)
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

                        html += "<option value='"+cidade.id+"'" + selected + ">" + cidade.cidade + "</option>";
                    }
                });

                $('#cidade').html(html);
            }
        });
    }
}

document.addEventListener('DOMContentLoaded', () => {
    if ($('#Id').val() == 0 || $('#Id').val() == '') {
        $('#Id').val('')
        $('#Id').removeAttr('readonly')
    }

    GetGrupo()
    Estados()
})