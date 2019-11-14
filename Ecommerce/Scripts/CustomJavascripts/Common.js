var readURL = function (input, destination) { 
    if (input.files && input.files[0]) {
        var reader = new FileReader();       
        reader.onload = function (e) {           
            $(destination).attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);      
    }
};

var imagesPreview = function (input, placeToInsertImagePreview) {
    if (input.files) {
        var filesAmount = input.files.length;

        for (i = 0; i < filesAmount; i++) {
            var reader = new FileReader();

            reader.onload = function (event) {
                $($.parseHTML('<img>')).attr("style", "height:100px; width:100px;margin:5px;").attr('src', event.target.result).appendTo(placeToInsertImagePreview);
            }
            reader.readAsDataURL(input.files[i]);
        }
    }
};