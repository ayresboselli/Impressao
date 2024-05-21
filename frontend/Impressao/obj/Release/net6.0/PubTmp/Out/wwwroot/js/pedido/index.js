document.addEventListener('DOMContentLoaded', () => {
    $('#tblPedido').DataTable({
        "order": [[0, 'desc']],
        "language": { "url": "//cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json" },
        "lengthMenu": [[100, 200, 500], [100, 200, 500]],
    });
})