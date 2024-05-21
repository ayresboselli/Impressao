import { GetCliente, GetTabelaPreco } from './utils.js'

document.addEventListener('DOMContentLoaded', () => {

    //Date and time picker
    $('#reservationdatetime').datetimepicker({
        format: 'DD/MM/YYYY HH:mm',
        language: 'pt-BR',
        icons: { time: 'far fa-clock' }
    });

    GetCliente()
    GetTabelaPreco()

    $('#ClienteId').on('change', () => {
        GetTabelaPreco()
    })
})