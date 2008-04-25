
function CallWebService() {  
    [REPLACEPROJECTNAME].HelloWebService.set_path(window.wsUrl);
    [REPLACEPROJECTNAME].HelloWebService.HelloWorld(WebServiceCallback);
}
  
function WebServiceCallback(result) {
    var display = document.getElementById("DisplaySpan");
    display.innerHTML = result;
}