/// <reference path="Ace/ace.js" />
/// <reference path="jquery.min.js" />
showTab("Raw");

// code to activate relevant tab
$(".formattingheader a").click(showTab);

function showTab(elOrName) {
    var tab = "";
    if (typeof(elOrName) === "string")
        tab = elOrName;
    else {
        var $el = $(this);
        tab = $el.data("id");     
    }
    if (!tab)
        return;


    $("pre[id^=ResponseOutput]").hide();
    $("#ResponseOutput" + tab).show();

    // button handling
    $(".formattingheader a").removeClass("active");
    $(".formattingheader a[data-id=" + tab + "]").addClass("active");
}



function configureAceEditor(serverVars) {
    var editor = ace.edit("ResponseOutputFormatted");
    var session = editor.getSession();
    editor.setTheme("ace/theme/" + serverVars.theme);
    //editor.setTheme("ace/theme/textmate");
    //editor.setTheme("ace/theme/clouds");
    //editor.setTheme("ace/theme/xcode");
    //editor.setTheme("ace/theme/eclipse");
    //editor.setTheme("ace/theme/mono_industrial");
    session.setMode("ace/mode/" + serverVars.language);
    editor.setFontSize(13);

    if (!serverVars.allowEdit) {
        editor.setReadOnly(true);
        editor.setHighlightActiveLine(false);
    } else
        editor.setHighlightActiveLine(true);
    editor.renderer.setShowGutter(serverVars.showLineNumbers);
    editor.renderer.setPadding(10);

    // fill entire view
    editor.setOptions({
        maxLines: Infinity,
        minLines: 100
    });

    editor.setShowPrintMargin(false);

    session.setTabSize(3);
    //editor.getSession().setUseWrapMode(true);
    //$("#CodeDisplay").css("opacity", "1");

    return editor;
}

// configure the ace Editor
//// server side variables persisted into JavaScript
//ScriptVariables scriptVars = new ScriptVariables(new TextBox(), "serverVars");
//scriptVars.Add("showLineNumbers", Model.Snippet.ShowLineNumbers);
//scriptVars.Add("allowEdit", Model.AllowEdit);
//scriptVars.Add("language", Model.Snippet.Language.ToLower());
//scriptVars.Add("baseUrl", Url.Content("~/"));
//scriptVars.Add("theme", Model.UserState.Theme);
//scriptVars.Add("callbackHandler", Url.Content("~/CodePasteHandler.ashx"));
// ace editor
//$("#ResponseOutputFormatted").show();
window.aceEditor = configureAceEditor(serverVars);
//aceEditor.getSession().setMode("ace/mode/" + serverVars.language);
//setTimeout(function () {
//    $("#ResponseOutputFormatted").hide();
//    console.log('hiding');
//}, 1000);