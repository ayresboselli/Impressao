let PedidoItemSituacao = [
    "Aguardando arquivos",
    "Processando arquivos",
    "Aguardando aprovação",
    "Gerando PDF",
    "Pronto para a impressão",
]

function GetCliente() {
    let id = $('#cliente').val()
    
    $.ajax({
        url: '/Pedido/Clientes',
        method: 'get',
        dataType: 'json',
        async: false,
        success: result => {
            let html = '<option value=""></option>';
            result.map(r => {
                let selected = ''
                if (r.id == id) {
                    selected = ' selected'
                }
                html += '<option value="' + r.id + '"' + selected + '>' + r.razaoSocial + '</option>'
            })
            $('#ClienteId').html(html)
        },
        error: result => {
            console.log('error', result)
        }
    })
}

function GetTabelaPreco() {
    let id = $('#tabela').val()
    let cliente = $('#ClienteId').val()

    $.ajax({
        url: '/Pedido/TabelasPrecos',
        method: 'post',
        dataType: 'json',
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            cliente
        },
        success: result => {
            let html = '';
            result.map(r => {
                let selected = ''
                if (r.id == id) {
                    selected = ' selected'
                }
                html += '<option value="' + r.id + '"' + selected + '>' + r.titulo + '</option>'
            })
            $('#TabelaId').html(html)
        },
        error: result => {
            console.log('error', result)
        }
    })
}

function GetProduto() {
    let id = $('#modalProdutoId').val()

    $.ajax({
        url: '/Produto/List',
        method: 'get',
        dataType: 'json',
        beforeSend: (jqXHR, settings) => {
            $('#modalProduto').html("<option><div class='spinner-border spinner-border-sm'></div></option>")
        },
        success: result => {
            let html = '<option value=""></option>';
            result.map(r => {
                if (id == '') {
                    html += '<option value="' + r.id + '">' + r.titulo + '</option>'
                } else {
                    if (r.id == id) {
                        html = '<option value="' + r.id + '">' + r.titulo + '</option>'
                    }
                }
                //let selected = ''
                //if (r.id == id) {
                //    selected = ' selected'
                //}
                //html += '<option value="' + r.id + '"' + selected + '>' + r.titulo + '</option>'
            })
            $('#modalProduto').html(html)
        },
        error: result => {
            console.log('error', result)
        }
    })
}

function GetPreco(produto) {
    let tabela = $('#TabelaId').val()
    //let produto = $('#modalProduto').val()

    $.ajax({
        url: '/Pedido/PrecoProduto/',
        method: 'post',
        dataType: 'json',
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            tabela,
            produto
        },
        success: result => {
            console.log(result)
            if (result.valor != undefined) {
                $('#modalPreco').val(parseFloat(result.valor).toFixed(2).replace('.',','))
            }
        },
        error: result => {
            console.log('error', result)
        }
    })
}

function GetItens() {
    const urlSubParam = window.location.href.split('//')[1].split('/')[2];
    let id = $('#Id').val()

    $.ajax({
        url: '/Pedido/Itens/' + id,
        method: 'get',
        dataType: 'json',
        beforeSend: (jqXHR, settings) => {
            $('#tbItens').html("<tr><td colspan='6' class='text-center'><div class='spinner-border spinner-border-sm'></div></td></tr>")
        },
        success: result => {
            let html = '';
            result.map(r => {
                let situacao = 0
                if (r.dataUpload == null) {
                    situacao = 0
                } else if (r.dataUpload != null && r.dataProcessamentoUpload == null) {
                    situacao = 1
                } else if (r.dataProcessamentoUpload != null && r.dataAprovacao == null) {
                    situacao = 2
                } else if (r.dataAprovacao != null && r.dataImposicao == null) {
                    situacao = 3
                } else if (r.dataImposicao != null) {
                    situacao = 4
                }

                let preco = ''
                if (r.preco != null) {
                    preco = r.preco.toFixed(2).replace('.',',')
                }

                html += "<tr>"
                html += "    <td>" + r.id + "</td>"
                html += "    <td>" + r.produto.id + " - " + r.produto.titulo + "</td>"
                html += "    <td>" + r.copias + "</td>"
                html += "    <td>" + parseInt(r.pedidoItemArquivos.length + r.pedidoItemUploads.length) + "</td>"
                html += "    <td>" + (r.index ? "<span class='text-success'>Sim</span>" : "<span class='text-danger'>Não</span>") + "</td>"
                html += "    <td>" + preco + "</td>"
                html += "    <td>" + PedidoItemSituacao[situacao] + "</td>"

                if (urlSubParam != 'View') {
                    html += "    <td>"
                    html += "        <a href='javascript:void(0)' onclick='ModalPedidoItem(" + r.id + "," + r.produtoId + "," + r.copias + "," + r.index + ")' title='Editar' class='text-primary mx-2'><i class='fas fa-pen'></i></a>"
                    if (situacao == 0) {
                        html += "        <div class='dropdown' style='display:inline'>"
                        html += "           <a href='' title='Upload' class='text-primary mx-2' data-toggle='dropdown'><i class='fas fa-upload'></i></a>"
                        html += "           <ul class='dropdown-menu'>"
                        html += "               <li class='dropdown-item'><a href = '/Pedido/UploadJpgFile/" + r.id + "'>Arquivos</a></li>"
                        html += "               <li class='dropdown-item'><a href = '/Pedido/UploadJpgFolder/" + r.id + "'>Pastas</a></li>"
                        html += "            </ul>"
                        html += "        </div>"
                    } else {
                        html += "        <a href='/Pedido/Arquivos/" + r.id + "' title='Arquivos' class='text-primary mx-2'><i class='fas fa-search'></i></a>"
                    }

                    if (situacao == 2) {
                        html += "        <a href='javascript:void(0)' onclick='Aprovar(" + r.id + ")' title='Aprovar' class='text-success mx-2'><i class='fas fa-print'></i></a>"
                    }

                    if (situacao > 2) {
                        html += "        <a href='/Pedido/ReportSpreadsheet/" + r.id + "' target='_blank' title='Planilha' class='text-success mx-2'><i class='fas fa-table'></i></a>"
                    }

                    html += "        <a href='javascript:void(0)' onclick='Reset(" + r.id + ")' title='Resetar' class='text-danger mx-2'><i class='fas fa-undo'></i></a>"

                    // html += "        <a href='/Pedido/AnalisaImagem/" + r.id + "' title='Arquivos' class='text-primary mx-2'><i class='fas fa-search'></i></a>"
                    html += "    </td>"
                }
                html += "</tr>"
            })

            if (html == '') {
                html = "<tr><td colspan='6' class='text-center'>Nenhum item encontrado</td></tr>"
            }

            $('#tbItens').html(html)
        },
        error: result => {
            console.log('error', result)
        }
    })
}

function ModalPedidoItem(id_item, id_produto, copias = 1, index = false, preco = '') {
    $('#modalProdutoId').val(id_produto)
    $('#modalIdItem').val(id_item)
    $('#modalPreco').val(preco)
    $('#modalCopias').val(copias)
    $('#modalIndex')[0].checked = index
    
    if (id_item != undefined) {
        $('#modalProduto').attr('readonly', 'readonly');
    } else {
        $('#modalProduto').removeAttr('readonly');
    }

    GetProduto()

    $('#modalPedidoItem').modal('show')
}

function SalvarItem() {
    let token = $('input[name="__RequestVerificationToken"]').val();
    let id = $('#Id').val()
    let idItem = $('#modalIdItem').val()
    let produto = $('#modalProduto').val()
    let preco = $('#modalPreco').val()
    let copias = $('#modalCopias').val()
    let index = $('#modalIndex')[0].checked
    let erro = false
    
    if (produto == '') {
        $('#modalProduto').addClass('is-invalid')
        $('#modalProdutoErro').show()
        erro = true
    }

    if (preco == '') {
        $('#modalPreco').addClass('is-invalid')
        $('#modalPrecoErro').show()
        erro = true
    }

    if (copias == '') {
        $('#modalCopias').addClass('is-invalid')
        $('#modalCopiasErro').show()
        erro = true
    }

    if (!erro) {
        $('#modalProduto').removeClass('is-invalid')
        $('#modalProdutoErro').hide()
        $('#modalCopias').removeClass('is-invalid')
        $('#modalCopiasErro').hide()

        $.ajax({
            url: '/Pedido/Itens',
            method: 'post',
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            data: {
                __RequestVerificationToken: token,
                Id: idItem,
                PedidoId: id,
                ProdutoId: produto,
                Preco: preco,
                Copias: copias,
                Index: index
            },
            success: result => {
                if (result.success) {
                    $('#modalPedidoItem').modal('hide')
                    Swal.fire('Salvo!', '', 'success')
                    GetItens()
                } else {
                    Swal.fire('Falhou!', result.msg, 'error')
                }
            },
            error: result => {
                console.log('error', result)
            }
        })
    }
}

function Aprovar(id) {
    Swal.fire({
        title: 'Tem certeza que deseja aprovar este item?',
        showCancelButton: true,
        confirmButtonText: 'Aprovar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#28a745',
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            $.ajax({
                url: "/Pedido/AprovarPedido",
                method: "post",
                data: {
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                    id
                },
                success: result => {
                    if (result.success) {
                        Swal.fire('Aprovado!', 'Os álbuns já estão na fila de imposição', 'success')
                        GetItens()
                    } else {
                        Swal.fire('Falhou!', result.msg, 'danger')
                    }
                },
                error: result => {
                    console.log('error', result)
                    Swal.fire('Falhou!', '', 'danger')
                }
            })

        }
    })
}

function Reset(id) {
    Swal.fire({
        title: 'Tem certeza que deseja resetar este item?',
        showCancelButton: true,
        confirmButtonText: 'Resetar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#dc3545',
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            $.ajax({
                url: "/Pedido/ResetItem",
                method: "post",
                data: {
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                    id
                },
                success: result => {
                    if (result.success) {
                        Swal.fire('Resetado!', '', 'success')
                        GetItens()
                    } else {
                        Swal.fire('Falhou!', result.msg, 'danger')
                    }
                },
                error: result => {
                    console.log('error', result)
                    Swal.fire('Falhou!', '', 'danger')
                }
            })
            
        }
    })
}

function Finalizar() {
    Swal.fire({
        title: 'Tem certeza que deseja finalizar este pedido?',
        showCancelButton: true,
        confirmButtonText: 'Finalizar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#dc3545',
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            $.ajax({
                url: "/Pedido/Finalizar",
                method: "post",
                data: {
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                    id: $('#Id').val()
                },
                success: result => {
                    if (result.success) {
                        Swal.fire('Finalizado!', '', 'success').then(() => {
                            location.href = '/Pedido'
                        })
                    } else {
                        Swal.fire('Falhou!', result.msg, 'danger')
                    }
                },
                error: result => {
                    console.log('error', result)
                    Swal.fire('Falhou!', '', 'danger')
                }
            })

        }
    })
}

document.addEventListener('DOMContentLoaded', () => {
    //Date and time picker
    $('#reservationdatetime').datetimepicker({
        format: 'DD/MM/YYYY HH:mm',
        language: 'pt-BR',
        icons: { time: 'far fa-clock' }
    });


    GetCliente()
    GetTabelaPreco()
    GetItens()

    $('#modalBtnSalvar').click(() => {
        SalvarItem()
    })

    $('#btnPdf').on('click', () => {
        window.open('/Pedido/ReportOrder/' + $('#Id').val())
    })

    $('#btnFinalizar').on('click', () => {
        Finalizar()
    })
})