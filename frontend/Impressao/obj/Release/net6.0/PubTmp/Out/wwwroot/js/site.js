function Menu()
{
    const urlParam = window.location.href.split('//')[1].split('/')[1];
    const urlSubParam = window.location.href.split('//')[1].split('/')[2];

    $('.nav-link').removeClass('active')
    $('.nav-link').removeClass('menu-open')

    switch (urlParam) {
        case 'Pedido':
            if (urlSubParam != 'Finalizados') {
                $('#MenuPedido').addClass('active');
                $('#MenuLiPedido').addClass('menu-open');
                $('#MenuPedidoAtivos').addClass('active');
            } else {
                $('#MenuPedido').addClass('active');
                $('#MenuLiPedido').addClass('menu-open');
                $('#MenuPedidoFinalizados').addClass('active');
            }
            break
        case 'PedidoReimpressao': $('#MenuPedidoReimpressao').addClass('active'); break
        case 'Cliente':
            $('#MenuCliente').addClass('active');
            $('#MenuLiCliente').addClass('menu-open');
            $('#MenuCadCliente').addClass('active');
            break
        case 'ClienteGrupo':
            $('#MenuCliente').addClass('active');
            $('#MenuLiCliente').addClass('menu-open');
            $('#MenuGrupoCliente').addClass('active');
            break
        case 'Produto':
            $('#MenuProduto').addClass('active');
            $('#MenuLiProduto').addClass('menu-open');
            $('#MenuCadProduto').addClass('active');
            break
        case 'ProdutoGrupo':
            $('#MenuProduto').addClass('active');
            $('#MenuLiProduto').addClass('menu-open');
            $('#MenuGrupoProduto').addClass('active');
            break
        case 'TabelaPreco':
            $('#MenuTabelaPreco').addClass('active');
            $('#MenuLiTabelaPreco').addClass('menu-open');
            $('#MenuCadTabelaPreco').addClass('active');
            break
        case 'Celula':
            $('#MenuLiPCP').addClass('menu-open');
            $('#MenuPCP').addClass('active');
            $('#MenuCadCelula').addClass('active');
            break
        case 'UnidadeMedida':
            $('#MenuLiPCP').addClass('menu-open');
            $('#MenuPCP').addClass('active');
            $('#MenuCadUnidadeMedida').addClass('active');
            break
        case 'UnidadeMedidaConversao':
            $('#MenuLiPCP').addClass('menu-open');
            $('#MenuPCP').addClass('active');
            $('#MenuCadUnidadeMedidaConversao').addClass('active');
            break
        case 'SetorEstoque':
            $('#MenuLiPCP').addClass('menu-open');
            $('#MenuPCP').addClass('active');
            $('#MenuCadSetorEstoque').addClass('active');
            break
        case 'MateriaPrima':
            $('#MenuLiPCP').addClass('menu-open');
            $('#MenuPCP').addClass('active');
            $('#MenuCadMateriaPrima').addClass('active');
            break
        case 'MateriaPrimaGrupo':
            $('#MenuLiPCP').addClass('menu-open');
            $('#MenuPCP').addClass('active');
            $('#MenuCadMateriaPrimaGrupo').addClass('active');
            break
        case 'Usuario':
            $('#MenuUsuario').addClass('active');
            $('#MenuLiUsuario').addClass('menu-open');
            $('#MenuCadUsuario').addClass('active');
            break
        case 'Perfil':
            $('#MenuUsuario').addClass('active');
            $('#MenuLiUsuario').addClass('menu-open');
            $('#MenuCadPerfil').addClass('active');
            break
        case 'Empresa':
            $('#MenuConfig').addClass('active');
            $('#MenuLiConfig').addClass('menu-open');
            $('#MenuConfigEmpresa').addClass('active');
            break
        case 'Config':
            $('#MenuConfig').addClass('active');
            $('#MenuLiConfig').addClass('menu-open');
            $('#MenuConfigParam').addClass('active');
            break
        default: $('#MenuHome').addClass('active'); break
    }
}

function AlterarSenha() {
    let senhaA = $('#senhaA').val()
    let senhaN = $('#senhaN').val()
    let senhaR = $('#senhaR').val()
    let erro = false

    $('#spanSenhaA').text('')
    $('#spanSenhaN').text('')
    $('#spanSenhaR').text('')

    if (senhaA.length == 0) {
        $('#spanSenhaA').text('Digite a senha antiga')
        erro = true
    }
    if (senhaN.length == 0) {
        $('#spanSenhaN').text('Digite a nova senha')
        erro = true
    }
    if (senhaN != senhaR) {
        $('#spanSenhaR').text('Senhas incompatíveis')
        erro = true
    }

    if (!erro) {
        $.ajax({
            url: "/Auth/ChangePassword",
            method: 'post',
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                senhaA,
                senhaN,
                senhaR
            },
            success: result => {
                if (result.success) {
                    Swal.fire('Salvo!', '', 'success')
                    $('#modalAltSenha').modal('hide')

                    $('#senhaA').val('')
                    $('#senhaN').val('')
                    $('#senhaR').val('')
                } else {
                    if (result.errorCode == 2) {
                        $('#spanSenhaA').text(result.msg)
                    } else {
                        Swal.fire('Falhou!', result.msg, 'error')
                    }
                }
            },
            error: result => {
                console.log('erro',result)
            }
        })
    }
}

function Infos() {
    $.ajax({
        url: "/Home/Infos",
        method: 'post',
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: result => {
            console.log(result)
            $('#userName').text(result.userName)
        },
        error: result => {
            console.log('erro', result)
        }
    })
}

function Decimal(valor, decimal = 2) {
    valor += ''
    let tmp = ''

    // filtra apenas números
    let remove_zero = true;
    for (let i = 0; i < valor.length; i++) {
        if ((remove_zero && valor.charAt(i) != 0) || !remove_zero) {
            remove_zero = false;
            if (valor.charCodeAt(i) >= 48 && valor.charCodeAt(i) <= 58) {
                tmp += valor.charAt(i)
            }
        }
    }

    // mantém o mínimo de caracteres, conforme  o decimal
    if (tmp == '') {
        tmp = '0';
    }

    if (tmp.length < decimal + 1) {
        let limit = decimal - tmp.length
        for (let i = 0; i <= limit; i++) {
            tmp = '0' + tmp
        }
    }

    // adiciona virgula na antipenultima casa
    let casas_ponto = tmp.length - 3 - decimal
    if (decimal > 0) {
        tmp = tmp.substring(0, tmp.length - decimal) + ',' + tmp.substring(tmp.length - decimal);
    }

    // adiciona ponto a cada 3 casas
    for (let i = casas_ponto; i > 0; i -= 3) {
        tmp = tmp.substring(0, i) + '.' + tmp.substring(i);
    }

    return tmp
}

document.addEventListener('DOMContentLoaded', () => {
    $('.datatable').DataTable({
        "language": { "url": "//cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json" },
        "lengthMenu": [[100, 200, 500], [100, 200, 500]],
    });

    $('#modalAltSenhaSalvar').on('click',() => {
        AlterarSenha()
    })

    $('.decimal').on('keyup', (e) => {
        let elem = e.currentTarget
        elem.value = Decimal(elem.value)
    })

    Infos()
    Menu()
})