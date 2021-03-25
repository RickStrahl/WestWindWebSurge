/// <reference path="Ace/ace.js" />
/// <reference path="jquery.min.js" />
showTab("Formatted", "response");
showTab("Formatted", "request");

var websurge = {
    application: null    
};

// code to activate relevant tab
$(".formattingResponseBody a").click(function (e) {
    var el = this;
    showTab(el, "response");    
});
$(".formattingRequestBody a").click(function (e) {
    var el = this;
    showTab(el, "request");
});
$("#RequestUrl").click(function(e) {
    websurge.application.navigatebrowser(this.href);    
    e.preventDefault();
});

// Collapse/restore content underneath headers
$(".code-collapse").click(function() {
   
    var button = $(this);
    var el = button.parents(".header").next("div.codewrapper");
    
    if (el.is(":visible")) {
        el.slideUp();
        button.removeClass("fa-minus-square");
        button.addClass("fa-plus-square");
    } else {
        button.removeClass("fa-plus-square");
        button.addClass("fa-minus-square");
        el.slideDown();
    }
})
.attr("title","Collapse or expand section");

function showTab(elOrName, type) {
    var prefix = "ResponseBody";
    if (type == "request")
        prefix = "RequestBody";
    	
    var tab = "";
    if (typeof(elOrName) === "string")
        tab = elOrName;
    else {
        var $el = $(elOrName);
        tab = $el.data("id");     
    }
    if (!tab)
        return;
		
    $("*[id^=" + prefix + "]").hide();
	
	
	var $page = $("#" +prefix + tab);
	if ($page.length == 0){
	   tab = "Raw";
	   $page = $("#" + prefix + tab);
	}

	$page.show();
    $page.parents("div.codewrapper").show();
    
    // button handling
    $(".formatting" + prefix + " a").removeClass("active");
    $(".formatting" + prefix + " a[data-id=" + tab + "]").addClass("active");    
}


function configureAceEditor(editor, serverVars) {
    var session = editor.getSession();

    editor.setTheme("ace/theme/" + serverVars.theme);

    editor.setOptions({readOnly: true, highlightActiveLine: false, highlightGutterLine: false});    
    editor.renderer.$cursorLayer.element.style.display = "none" // hide cursor
    

    if (serverVars.allowEdit) {            
        editor.on("focus", function() {
            editor.setOptions({readOnly: false, highlightActiveLine: true, highlightGutterLine: true});
            editor.renderer.$cursorLayer.element.style.display = "block"
        });

        // Notify WPF of focus change
        editor.on("blur", function() { 
            
            editor.setOptions({readOnly: true, highlightActiveLine: false, highlightGutterLine: false});
            editor.renderer.$cursorLayer.element.style.display = "none" // hide cursor

            var text = editor.getSession().getValue();                                  
            if (websurge.application) {                         
                websurge.application.updatefromeditor(text, editor.id);
            }
        });
    }

    editor.renderer.setShowGutter(serverVars.showLineNumbers);
    editor.renderer.setPadding(10);

    session.setNewLineMode("windows");
    session.setUseWrapMode(true);

    // fill entire view
    editor.setOptions({
        maxLines: Infinity,
        minLines: 2
    });

    editor.setShowPrintMargin(false);

    session.setTabSize(3);
    //editor.getSession().setUseWrapMode(true);
    return editor;
}

//setTimeout(function() {
// attach ace to formatted code controls if they are loaded and visible

var lang = serverVars.requestLanguage;
if (lang == "urlencoded")
    lang = "text";

try {    
    window.aceEditorRequest = ace.edit("RequestHeaders");
    window.aceEditorRequest.id = "RequestHeaders";
    serverVars.allowEdit = true;
    configureAceEditor(aceEditorRequest, serverVars);
    aceEditorRequest.getSession().setMode("ace/mode/http");
}
catch(e) { }

try {    
    window.aceEditorRequest = ace.edit("RequestBodyFormatted");
    window.aceEditorRequest.id = "RequestBodyFormatted";
    serverVars.allowEdit = true;
    configureAceEditor(aceEditorRequest, serverVars);
    aceEditorRequest.getSession().setMode("ace/mode/" + lang);
}
catch(e) { }    


try {    
    window.aceEditorRequest = ace.edit("ResponseHeaders");
    window.aceEditorRequest.id = "ResponseHeaders";
    serverVars.allowEdit = false;
    configureAceEditor(aceEditorRequest, serverVars);
    aceEditorRequest.getSession().setMode("ace/mode/http");
}
catch(e) { }

setTimeout(function() {
    try {
        window.aceEditor = ace.edit("ResponseBodyFormatted");
        window.aceEditor.id = "ResponseBodyFormatted";
        serverVars.allowEdit = false;
        configureAceEditor(aceEditor, serverVars);
        aceEditor.getSession().setMode("ace/mode/" + serverVars.responseLanguage);
    } 
    catch(e) { }
},50);


//}, 0);


// Pass WebSurge AceInterop object into the browser so we can edit
function initializeInterop(webSurgeAceInterop) {    
    if (webSurgeAceInterop)
        websurge.application = webSurgeAceInterop;
}


//setTimeout(function () {
//    $("#ResponseOutputFormatted").hide();
//    console.log('hiding');
//}, 1000);