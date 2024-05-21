var token = null

function ValidaId(id) {
    $.ajax({
        url: '/Produto/ValidaId/' + id,
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

function GetGrupo() {
    let id = $('#grupo').val()
    
    $.ajax({
        url: '/Produto/Grupos',
        method: 'get',
        dataType: 'json',
        success: result => {
            let html = '';
            result.map(r => {
                let selected = ''
                if (r.id == id) {
                    selected = ' selected'
                }
                html += '<option value="' + r.id + '"' + selected + '>' + r.titulo + '</option>'
            })
            if (html == '') {
                html = '<option value=""></option>'
            }
            $('#ProdutoGrupoId').html(html)
        },
        error: result => {
            console.log('error', result)
        }
    })
}

//function DivImpFotos() {
//    if ($('#ArquivosJPEG')[0].checked) {
//        $('#DivImpFotos').show()
//    } else {
//        $('#DivImpFotos').hide()
//    }
//}

function DispImpressao() {
    let disp = $('#DisposicaoImpressao').val();
    if (disp == 'S') {
        $('#DisposicaoImagem').attr('readonly', 'readonly')
        $('#DivVerso').hide()
    } else {
        $('#DisposicaoImagem').removeAttr('readonly')
        $('#DivVerso').show()
    }
}

function DispImagem() {
    console.log($('#DisposicaoImagem'))
    Imagem()
}

function Midia() {
    let width = parseInt($('#cardFrente').width())
    let larg = parseInt($('#LarguraMidia').val())
    let alt = parseInt($('#AlturaMidia').val())

    proporcao = width / larg

    let style = 'width: ' + (larg * proporcao) + 'px;'
    style += 'height: ' + (alt * proporcao) + 'px;'

    $('.dvImpMidia').attr('style', style)

    Imagem()
}

function Imagem() {
    $('.dvImpImagem').remove()
    ImagemFrente()
    if ($('#DisposicaoImpressao').val() == 'D' && $('#DisposicaoImagem').val() == 'D') {
        ImagemVerso()
    }
}

function ImagemFrente() {
    let larg = parseInt($('#Largura').val()) * proporcao
    let alt = parseInt($('#Altura').val()) * proporcao
    let deslX = parseInt($('#DeslocamentoFrenteX').val()) * proporcao
    let deslY = parseInt($('#DeslocamentoFrenteY').val()) * proporcao
    let deslFX = 0
    let deslFY = 0

    let imagem = document.createElement('div')
    imagem.id = 'imagemF'
    imagem.className = 'dvImpImagem'
    imagem.style.marginTop = deslY + 'px'
    imagem.style.marginLeft = deslX + 'px'

    if ($('#Orientacao').val() == 0 || $('#Orientacao').val() == 180) {
        imagem.style.width = larg + 'px'
        imagem.style.height = alt + 'px'
        deslFX = parseInt($('#LarguraMidia').val()) - parseInt($('#Largura').val()) - parseInt($('#DeslocamentoFrenteX').val() || 0)
        deslFY = parseInt($('#AlturaMidia').val()) - parseInt($('#Altura').val()) - parseInt($('#DeslocamentoFrenteY').val() || 0)
    } else {
        imagem.style.width = alt + 'px'
        imagem.style.height = larg + 'px'
        deslFX = parseInt($('#LarguraMidia').val()) - parseInt($('#Altura').val()) - parseInt($('#DeslocamentoFrenteX').val() || 0)
        deslFY = parseInt($('#AlturaMidia').val()) - parseInt($('#Largura').val()) - parseInt($('#DeslocamentoFrenteY').val() || 0)
    }
    $('#deslFX').text(deslFX)
    $('#deslFY').text(deslFY)

    $('#imagemF').remove()
    $('#DvMidiaFrente').append(imagem)
}

function ImagemVerso() {
    let larg = parseInt($('#Largura').val()) * proporcao
    let alt = parseInt($('#Altura').val()) * proporcao
    let deslX = parseInt($('#DeslocamentoVersoX').val()) * proporcao
    let deslY = parseInt($('#DeslocamentoVersoY').val()) * proporcao
    let deslVX = 0
    let deslVY = 0

    let imagem = document.createElement('div')
    imagem.id = 'imagemV'
    imagem.className = 'dvImpImagem'
    imagem.style.marginTop = deslY + 'px'
    imagem.style.marginLeft = deslX + 'px'

    if ($('#Orientacao').val() == 0 || $('#Orientacao').val() == 180) {
        imagem.style.width = larg + 'px'
        imagem.style.height = alt + 'px'
        deslVX = parseInt($('#LarguraMidia').val()) - parseInt($('#Largura').val()) - parseInt($('#DeslocamentoVersoX').val() || 0)
        deslVY = parseInt($('#AlturaMidia').val()) - parseInt($('#Altura').val()) - parseInt($('#DeslocamentoVersoY').val() || 0)
    } else {
        imagem.style.width = alt + 'px'
        imagem.style.height = larg + 'px'
        deslVX = parseInt($('#LarguraMidia').val()) - parseInt($('#Altura').val()) - parseInt($('#DeslocamentoVersoX').val() || 0)
        deslVY = parseInt($('#AlturaMidia').val()) - parseInt($('#Largura').val()) - parseInt($('#DeslocamentoVersoY').val() || 0)
    }

    $('#deslVX').text(deslVX)
    $('#deslVY').text(deslVY)

    $('#imagemV').remove()
    $('#DvMidiaVerso').append(imagem)
}


/****** Informações ******/
function CriaElementoInfo(info) {
    let div = $("<div />")
    let top = 0
    let left = 0

    div.addClass('impInformacao')
    switch (info.tipo) {
        case 'barras': div.addClass('infoBarcode'); break
        case 'texto': div.addClass('infoText'); break
    }

    div.attr({
        style: "top: " + parseInt(info.posY * proporcao) + "px; left: " + parseInt(info.posX * proporcao) + "px; transform: rotate(" + info.orientacao + "deg);"
    });
    
    if (info.pagina == 0) {
        $('#DvMidiaFrente').append(div)
    } else {
        $('#DvMidiaVerso').append(div)
    }

}

function ListarInfo() {
    $('.impInformacao').remove()

    $.ajax({
        url: '/Produto/ListInfo/' + $('#Id').val(),
        method: 'get',
        success: result => {
            var htmlFrente = "";
            var htmlVerso = "";

            result.map(r => {
                CriaElementoInfo(r);

                var tipo = '';
                switch (r.tipo) {
                    case 'barras': tipo = 'Código de barras'; break;
                    case 'texto': tipo = 'Texto'; break;
                }

                var orientacao = '';
                switch (r.orientacao) {
                    case 0: orientacao = 'Nenhuma'; break;
                    case 90: orientacao = 'À esquerda'; break;
                    case 270: orientacao = 'À direita'; break;
                    case 180: orientacao = 'Para baixo'; break;
                }

                if (r.pagina == 0) {
                    htmlFrente += "<tr><td>" + tipo + "</td><td>" + r.texto + "</td><td>" + orientacao + "</td><td>";
                    htmlFrente += "<a href='javascript:void(0)' onclick='EditInfo(\"" + r.id + "\")' title='Editar' class='mx-2'><i class='fas fa-pen'></i></a>";
                    htmlFrente += "<a href='javascript:void(0)' onclick='DeleteInfo(\"" + r.id + "\")' title='Deletar' class='text-danger mx-2'><i class='fas fa-trash'></i></a>";
                    htmlFrente += "</td></tr>";
                } else {
                    htmlVerso += "<tr><td>" + tipo + "</td><td>" + r.texto + "</td><td>" + orientacao + "</td><td>";
                    htmlVerso += "<a href='javascript:void(0)' onclick='EditInfo(\"" + r.id + "\")' title='Editar' class='mx-2'><i class='fas fa-pen'></i></a>";
                    htmlVerso += "<a href='javascript:void(0)' onclick='DeleteInfo(\"" + r.id + "\")' title='Deletar' class='text-danger mx-2'><i class='fas fa-trash'></i></a>";
                    htmlVerso += "</td></tr>";
                }
            });

            if (htmlFrente.length == 0) {
                htmlFrente = "<tr><td colspan='4'>Nenhuma informação cadastrada</td></tr>";
            }

            if (htmlVerso.length == 0) {
                htmlVerso = "<tr><td colspan='4'>Nenhuma informação cadastrada</td></tr>";
            }

            $('#InfoFrente').html(htmlFrente);
            $('#InfoVerso').html(htmlVerso);
        }
    });
}

function AddInfo(fv) {
    //frenteVerso = fv;

    $('#InfoPagina').val(fv);
    $('#InfoId').val('');
    $('#InfoTipo').val('texto');
    $('#InfoTexto').val('');
    $('#InfoPosX').val(0);
    $('#InfoPosY').val(0);
    $('#InfoOrientacao').val(0);

    $('#modalInfo').modal('show');
}

function EditInfo(id) {
    $.ajax({
        url: '/Produto/Info/' + id,
        method: 'get',
        success: result => {
            console.log(result)
            //frente_verso = result.pagina;
            $('#InfoPagina').val(result.pagina);
            $('#InfoId').val(result.id);
            $('#InfoTipo').val(result.tipo);
            $('#InfoTexto').val(result.texto);
            $('#InfoPosX').val(result.posX);
            $('#InfoPosY').val(result.posY);
            $('#InfoOrientacao').val(result.orientacao);

            $('#modalInfo').modal('show');
        }
    });
}

function InfoSalvar() {
    let token = $('input[name="__RequestVerificationToken"]').val();
    let dados = {
        __RequestVerificationToken: token,
        Id: $('#InfoId').val(),
        ProdutoId: $('#Id').val(),
        Pagina: $('#InfoPagina').val(),
        Tipo: $('#InfoTipo').val(),
        Texto: $('#InfoTexto').val(),
        PosX: $('#InfoPosX').val(),
        PosY: $('#InfoPosY').val(),
        Orientacao: $('#InfoOrientacao').val()
    }

    $.ajax({
        url: '/Produto/Info',
        method: 'post',
        data: dados,
        success: result => {
            if (result.success) {
                toastr.success('Informação salva com sucesso');
                ListarInfo();
            } else {
                toastr.error(result.msg);
            }

            $('#modalInfo').modal('hide');
        },
        error: function (result) {
            console.log('Erro', result);
            toastr.error('Erro ao salvar a informação');

            $('#modalInfo').modal('hide');
        }
    });
}

function DeleteInfo(id) {
    let token = $('input[name="__RequestVerificationToken"]').val();

    Swal.fire({
        title: 'Tem certeza que deseja excluir esta informação?',
        showCancelButton: true,
        confirmButtonText: 'Deletar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#dc3545'
    }).then((result) => {
        if (result.isConfirmed) {
            
            $.ajax({
                url: '/Produto/InfoDelete/'+id,
                method: 'post',
                data: {
                    __RequestVerificationToken: token,
                },
                success: result => {
                    if (result.success) {
                        Swal.fire('Excluído!', '', 'success')
                        ListarInfo();
                    } else {
                        toastr.error(result.msg);
                    }
                },
                error: function (result) {
                    console.log('Erro', result);
                    toastr.error('Erro ao excluir a informação');

                    $('#modalInfo').modal('hide');
                }
            });
        }
    })
}


/****** Engenharia ******/
function ListarCelulas() {
    $.ajax({
        url: '/Produto/ListarCelulas/' + $('#Id').val(),
        method: 'get',
        success: result => {
            console.log(result)
            let html = ""
            result.map(r => {
                html += "<div class='list-group-item'>"
                html += "    <input type='hidden' class='id_roteiros' value='" + r.id + "'>"
                html += '    <a href="javascript:void(0)" style="color:#000" data-toggle="collapse" data-target="#listaCelula' + r.id + '">' + r.celula.titulo + '</a>'
                html += '    <div id="listaCelula' + r.id + '" class="collapse">'
                html += '        <div class="my-2">'
                html += '            <a class="btn btn-sm btn-secondary">Adicionar matéria prima</a>'
                html += '        </div>'
                html += '        <table class="table table-sm table-bordered table-striped">'
                html += '            <thead>'
                html += '                <tr>'
                html += '                    <th>Matéria Prima</th>'
                html += '                    <th>Unidade</th>'
                html += '                    <th>Quantidade</th>'
                html += '                    <th></th>'
                html += '                </tr>'
                html += '            </thead>'
                html += '            <tbody>'
                html += '                <tr colspan="4">'
                html += '                    <td>Nenhum item</td>'
                html += '                </tr>'
                html += '            </tbody>'
                html += '        </table>'
                html += '    </div>'
                html += "</div>"
            });

            $('#listaCelulas').html(html)
        }
    });
}

function ModalAddCelula() {
    $.ajax({
        url: '/Produto/ListarCelulasAdd/' + $('#Id').val(),
        method: 'get',
        success: result => {
            let html = "<option></option>"
            result.map(r => {
                html += "<option value='" + r.id + "'>" + r.titulo +"</option>"
            });
            
            $('#modalAddCelulaSlc').html(html)
        }
    });

    $('#modalAddCelula').modal('show')
}

function ModalAddCelulaAdd() {
    $.ajax({
        url: '/Produto/CelulasAdd',
        method: 'post',
        data: {
            __RequestVerificationToken: token,
            IdProduto: $('#Id').val(),
            IdCelula: $('#modalAddCelulaSlc').val()
        },
        success: result => {
            if (result != "") {
                ListarCelulas()
                $('#modalAddCelula').modal('hide')
                toastr.success("Célula adicionada com sucesso")
            } else {
                toastr.error("Erro ao adicionar a célula")
            }
        }
    });
}

let proporcao = 0
let frenteVerso = 0;

document.addEventListener('DOMContentLoaded', () => {
    token = $('input[name="__RequestVerificationToken"]').val();

    if ($('#Id').val() == 0 || $('#Id').val() == '') {
        $('#Id').val('')
        $('#Id').removeAttr('readonly')
        $('#form').attr('action','/Produto/Edit/?id=0')
    } else {
        $('#dvImposicao').show()
    }

    GetGrupo()
    //DivImpFotos()
    DispImpressao()
    Midia()
    ListarInfo()

    ListarCelulas()

    new Sortable(document.getElementById('listaCelulas'), {
        animation: 150,
        ghostClass: 'blue-background-class'
    });

    $('#btnAddCelula').on('click', () => {
        ModalAddCelula()
    })

    $('#modalAddCelulaAdd').on('click', () => {
        ModalAddCelulaAdd()
    })

    $('form').on('submit', () => {

        let itens = $('.id_roteiros')
        let lista = []
        for (let i = 0; i < itens.length; i++) {
            lista.push(itens[i].value)
        }
        
        $.ajax({
            url: '/Produto/RoteiroSequencia/',
            method: 'post',
            async: false,
            data: {
                __RequestVerificationToken: token,
                ProdutoId: $('#Id').val(),
                Ordem: lista
            },
            success: result => {
                console.log(result)
            }
        });
    })

})