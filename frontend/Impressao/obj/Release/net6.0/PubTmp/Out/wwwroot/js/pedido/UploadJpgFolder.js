function FinalizarUpload() {
    let token = $('input[name="__RequestVerificationToken"]').val();
    let id = $('#Id').val()
    let idPedido = $('#Pedido_Id').val()

    $.ajax({
        url: '/Pedido/FinalizarUpload',
        method: 'post',
        dataType: 'json',
        data: {
            __RequestVerificationToken: token,
            id
        },
        success: result => {
            if (result.success) {
                Swal.fire('Upload finalizado com sucesso!', '', 'success').then(() => {
                    location.href = "/Pedido/Edit/"+idPedido
                })
            } else {
                Swal.fire('Falhou!', result.msg, 'error')
            }
        },
        error: result => {
            console.log('error', result)
        }
    })
}

document.addEventListener('DOMContentLoaded', () => {
    let token = $('input[name="__RequestVerificationToken"]').val();
    let enviado = false;

    // Get the template HTML and remove it from the doumenthe template HTML and remove it from the doument
    var previewNode = document.querySelector("#template");
    previewNode.id = "";
    var previewTemplate = previewNode.parentNode.innerHTML;
    previewNode.parentNode.removeChild(previewNode);

    var myDropzone = new Dropzone(".dropzoneContent", { // Make the whole body a dropzone
        //url: "/Pedido/UploadJpg", // Set the url
        url: "/Pedido/Upload", // Set the url
        //thumbnailWidth: 80,
        //thumbnailHeight: 80,
        createImageThumbnails: false,
        parallelUploads: 10,
        previewTemplate: previewTemplate,
        autoQueue: false, // Make sure the files aren't queued until manually added
        previewsContainer: "#previews", // Define the container to display the previews
        clickable: ".fileinput-button", // Define the element that should be used as click trigger to select files.
        acceptedFiles: "image/jpeg",
        method: "post",
        init: function () {
            this.hiddenFileInput.setAttribute("webkitdirectory", true);
        },
        success: function (file, response) {
            this.removeFile(file);
        },
        error: function (file, response) {
            console.log('erro', response);
        }
    });

    myDropzone.on("addedfile", function (file) {
        // Hookup the start button
        //file.previewElement.querySelector(".start").onclick = function () { myDropzone.enqueueFile(file); };
    });

    // Update the total progress bar
    myDropzone.on("totaluploadprogress", function (progress) {
        //document.querySelector("#total-progress .progress-bar").style.width = progress + "%";
    });

    myDropzone.on("sending", function (file, xhr, formData) {
        enviado = true
        formData.append('id_item', $("#Id").val());
        formData.append('path', file.webkitRelativePath);
        formData.append('__RequestVerificationToken', token);

        // Show the total progress bar when upload starts
        //document.querySelector("#total-progress").style.opacity = "1";
        // And disable the start button
        //file.previewElement.querySelector(".start").setAttribute("disabled", "disabled");
    });

    // Hide the total progress bar when nothing's uploading anymore
    myDropzone.on("queuecomplete", function (progress) {
        ////document.querySelector("#total-progress").style.opacity = "0";

        if (enviado) {
            Swal.fire({
                icon: 'success',
                title: 'Arquivos enviados',
                text: 'Deseja finalizar o upload?',
                showCancelButton: true,
                confirmButtonText: 'Finalizar',
                confirmButtonColor: '#007bff',
                cancelButtonColor: '#d33',
            }).then((result) => {
                if (result.isConfirmed) {
                    FinalizarUpload()
                } else {
                    location.href = "/Pedido/Edit/" + $('#Pedido_Id').val()
                }
            })
        }
    });

    // Setup the buttons for all transfers
    // The "add files" button doesn't need to be setup because the config
    // `clickable` has already been specified.
    document.querySelector("#actions .start").onclick = function () {
        myDropzone.enqueueFiles(myDropzone.getFilesWithStatus(Dropzone.ADDED));
    };
    document.querySelector("#actions .cancel").onclick = function () {
        myDropzone.removeAllFiles(true);
    };

    if ($('#PedidoItemUploads_Count').val() > 0) {
        $('#btnFinalizarUpload').show()
    }

    $('#btnFinalizarUpload').on('click', () => { FinalizarUpload() })
})