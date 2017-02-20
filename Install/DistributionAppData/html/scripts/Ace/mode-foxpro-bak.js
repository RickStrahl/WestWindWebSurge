/* ***** BEGIN LICENSE BLOCK *****
 * Distributed under the BSD license:
 *
 * Copyright (c) 2012, Ajax.org B.V.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of Ajax.org B.V. nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL AJAX.ORG B.V. BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *
 * Contributor(s):
 * 
 *
 *
 * ***** END LICENSE BLOCK ***** */

define('ace/mode/foxpro', ['require', 'exports', 'module' , 'ace/lib/oop', 'ace/mode/text', 'ace/tokenizer', 'ace/mode/foxpro_highlight_rules'], function(require, exports, module) {


var oop = require("../lib/oop");
var TextMode = require("./text").Mode;
var Tokenizer = require("../tokenizer").Tokenizer;    
var FoxProHighlightRules = require("./foxpro_highlight_rules").FoxProHighlightRules;

var Mode = function () {
    this.HighlightRules = FoxProHighlightRules;
};
oop.inherits(Mode, TextMode);

(function() {
       
    this.lineCommentStart = ["*", "&&"];
    
    this.$id = "ace/mode/foxpro";
}).call(Mode.prototype);

exports.Mode = Mode;
});


define('ace/mode/foxpro_highlight_rules', ['require', 'exports', 'module' , 'ace/lib/oop', 'ace/mode/text_highlight_rules'], function(require, exports, module) {


var oop = require("../lib/oop");
var TextHighlightRules = require("./text_highlight_rules").TextHighlightRules;

var FoxProHighlightRules = function() {

    var builtinFunctions = (
        "Abs|Aclass|Acopy|Acos|Adatabases|Adbobjects|Addbs|Addproperty|Adel|Adir|Adlls|Adockstate|Aelement|Aerror|Aevents|Afields|Afont|Agetclass|Agetfileversion|Ains|Ainstance|Alanguage|Alen|Alias|Alines|Alltrim|Amembers|Amouseobj|Anetresources|Aprinters|Aprocinfo|Asc|Ascan|Aselobj|Asessions|Asin|Asort|Astackinfo|Asubscript|At|At_c|Ataginfo|Atan|Atc|Atcc|Atcline|Atline|Atn2|Aused|Avcxclasses|" + 
        "Bar|Barcount|Barprompt|Between|Bindevent|Bintoc|Bitand|Bitclear|Bitlshift|Bitnot|Bitor|Bitrshift|Bitset|Bittest|Bitxor|Bof|" +
        "Candidate|Capslock|Cdow|Cdx|Ceiling|Chr|Chrsaw|Chrtran|Chrtranc|Cmonth|Cntbar|Cntpad|Col|Com|Comarray|Comclassinfo|Compobj|Comprop|Comreturnerror|Cos|Cpconvert|Cpcurrent|Cpdbf|Createbinary|" +
        "Createobject|Createobjectex|Createoffline|Ctobin|Ctod|Ctot|Curdir|Cursorgetprop|Cursorsetprop|Cursortoxml|Curval|" +
        ""
    );

    var constants = (
        ""
    );

    var keywords = (
        "Function|If|Else|EndIf|When|DO|EndFor|For|"
    );

    var keywordMapper = this.createKeywordMapper({
        "support.function": builtinFunctions,
        "keyword": keywords,
        "constant.language": constants
    }, "identifier", true);

    this.$rules = {
    "start": [
        {
            token: [
                "meta.ending-space"
            ],
            regex: "$"
        },
        {
            token: [
                null
            ],
            regex: "^(?=\\t)",
            next: "state_3"
        },
        {
            token: [null],
            regex: "^(?= )",
            next: "state_4"
        },
        {
            token: [
                "text",
                "storage.type.function.asp",
                "text",
                "entity.name.function.asp",
                "text",
                "punctuation.definition.parameters.asp",
                "variable.parameter.function.asp",
                "punctuation.definition.parameters.asp"
            ],
            regex: "^(\\s*)(function|func|procedure|proc)(\\s*)([a-zA-Z_]\\w*)(\\s*)(\\()([^)]*)(\\))"
        },
        {
            token: "punctuation.definition.comment.asp",
            regex: "\\*|&&",
            next: "comment"
        },
        {
            token: [
                "keyword.control.asp"
            ],
            regex: "\\b(?:IF|THEN|ELSE|ENDIF|DO|WHILE|ENDFOR|ENDDO|FOR|TO|EACH|CASE|SELECT|ENDCASE|ENDFUNC|ENDCLASS|RETURN|LOOP|NEXT|WITH|EXIT|IIF|FUNCTION|PROCEDURE|LOCAL|AS)\\b"
        },
        {
            token: "keyword.operator.asp",
            regex: "\\b(?:Mod|And|Not|Or|Xor|as)\\b"
        },
        {
            token: "storage.type.asp",
            regex: "THIS|CLASS|Dim|Call|Class|Const|Dim|Redim|Function|Sub|Private Sub|Public Sub|End sub|End Function|Set|Let|Get|New|Randomize|Option Explicit|On Error Resume Next|On Error GoTo"
        },
        {
            token: "storage.modifier.asp",
            regex: "\\b(?:PRIVATE|PUBLIC|PROTECTED)\\b"
        },
        {
            token: "constant.language.asp",
            regex: "\\b(?:Empty|False|Nothing|Null|True)\\b"
        },
        {
            token: "punctuation.definition.string.begin.asp",
            regex: '"',
            next: "string"
        },
        {
            token: "punctuation.definition.string.begin.asp",
            regex: "'",
            next: "string"
        },
        {
            token: "punctuation.definition.string.begin.asp",
            regex: "\\[",
            next: "string"
        },
        {
            token: [
                "punctuation.definition.variable.asp"
            ],
            regex: "(\\$)[a-zA-Z_x7f-xff][a-zA-Z0-9_x7f-xff]*?\\b\\s*"
        },
        {
            token: "support.class.asp",
            regex: "\\b(?:Application|ObjectContext|Request|Response|Server|Session)\\b"
        },
        {
            token: "support.class.collection.asp",
            regex: "\\b(?:Contents|StaticObjects|ClientCertificate|Cookies|Form|QueryString|ServerVariables)\\b"
        },
        {
            token: "support.constant.asp",
            regex: "\\b(?:TotalBytes|Buffer|CacheControl|Charset|ContentType|Expires|ExpiresAbsolute|IsClientConnected|PICS|Status|ScriptTimeout|CodePage|LCID|SessionID|Timeout)\\b"
        },
        {
            token: "support.function.asp",
            regex: "\\b(?:Lock|Unlock|SetAbort|SetComplete|BinaryRead|AddHeader|AppendToLog|BinaryWrite|Clear|End|Flush|Redirect|Write|CreateObject|HTMLEncode|MapPath|URLEncode|Abandon|Convert|Regex)\\b"
        },
        {
            token: "support.function.event.asp",
            regex: "\\b(?:Application_OnEnd|Application_OnStart|OnTransactionAbort|OnTransactionCommit|Session_OnEnd|Session_OnStart)\\b"
        },
        {
            token: "support.function.vb.asp",
            regex: "\\b(?:Array|Add|Asc|Atn|CBool|CByte|CCur|CDate|CDbl|Chr|CInt|CLng|Conversions|Cos|CreateObject|CSng|CStr|Date|DateAdd|DateDiff|DatePart|DateSerial|DateValue|Day|Derived|Math|Escape|Eval|Exists|Exp|Filter|FormatCurrency|FormatDateTime|FormatNumber|FormatPercent|GetLocale|GetObject|GetRef|Hex|Hour|InputBox|InStr|InStrRev|Int|Fix|IsArray|IsDate|IsEmpty|IsNull|IsNumeric|IsObject|Item|Items|Join|Keys|LBound|LCase|Left|Len|LoadPicture|Log|LTrim|RTrim|Trim|Maths|Mid|Minute|Month|MonthName|MsgBox|Now|Oct|Remove|RemoveAll|Replace|RGB|Right|Rnd|Round|ScriptEngine|ScriptEngineBuildVersion|ScriptEngineMajorVersion|ScriptEngineMinorVersion|Second|SetLocale|Sgn|Sin|Space|Split|Sqr|StrComp|String|StrReverse|Tan|Time|Timer|TimeSerial|TimeValue|TypeName|UBound|UCase|Unescape|VarType|Weekday|WeekdayName|Year)\\b"
        },
        {
            token: [
                "constant.numeric.asp"
            ],
            regex: "-?\\b(?:(?:0(?:x|X)[0-9a-fA-F]*)|(?:(?:[0-9]+\\.?[0-9]*)|(?:\\.[0-9]+))(?:(?:e|E)(?:\\+|-)?[0-9]+)?)(?:L|l|UL|ul|u|U|F|f)?\\b"
        },
        {
            token: "support.type.vb.asp",
            regex: "\\b(?:vbtrue|vbfalse|vbcr|vbcrlf|vbformfeed|vblf|vbnewline|vbnullchar|vbnullstring|int32|vbtab|vbverticaltab|vbbinarycompare|vbtextcomparevbsunday|vbmonday|vbtuesday|vbwednesday|vbthursday|vbfriday|vbsaturday|vbusesystemdayofweek|vbfirstjan1|vbfirstfourdays|vbfirstfullweek|vbgeneraldate|vblongdate|vbshortdate|vblongtime|vbshorttime|vbobjecterror|vbEmpty|vbNull|vbInteger|vbLong|vbSingle|vbDouble|vbCurrency|vbDate|vbString|vbObject|vbError|vbBoolean|vbVariant|vbDataObject|vbDecimal|vbByte|vbArray)\\b"
        },
        {
            token: [
                "entity.name.function.asp"
            ],
            regex: "(?:(\\b[a-zA-Z_x7f-xff][a-zA-Z0-9_x7f-xff]*?\\b)(?=\\(\\)?))"
        },
        {
            token: [
                "keyword.operator.asp"
            ],
            regex: "\\-|\\+|\\*\\\/|\\>|\\<|\\=|\\&"
        }
        //{
        //    token: "string.quoted.bracket.asp",           // [ string ]
        //    regex: "\\[.*?\\]"
        //},
        //{
        //    token: "string.quoted.single.asp",           // ' string '
        //    regex: "'*?'"
        //}

    ],
    "end":[
                {
                    token: "punctuation.definition.string.begin.asp",
                    regex: "\\]",
                    next: "string"
                }
    ],
    "state_3": [
        {
            token: [
                "meta.odd-tab.tabs",
                "meta.even-tab.tabs"
            ],
            regex: "(\\t)(\\t)?"
        },
        {
            token: "meta.leading-space",
            regex: "(?=[^\\t])",
            next: "start"
        },
        {
            token: "meta.leading-space",
            regex: ".",
            next: "state_3"
        }
    ],
    "state_4": [
        {
            token: ["meta.odd-tab.spaces", "meta.even-tab.spaces"],
            regex: "(  )(  )?"
        },
        {
            token: "meta.leading-space",
            regex: "(?=[^ ])",
            next: "start"
        },
        {
            defaultToken: "meta.leading-space"
        }
    ],
    "comment": [
        {
            token: "comment.line.apostrophe.asp",
            regex: "$|(?=(?:%>))",
            next: "start"
        },
        {
            defaultToken: "comment.line.apostrophe.asp"
        }
    ],
    "string": [
        {
            token: "constant.character.escape.apostrophe.asp",
            regex: '""'
        },
        {
            token: "string.quoted.bracket.asp",
            regex: "\\[",
            next: "start"
        },
        {
            token: "string.quoted.bracket.asp",
            regex: "\\]",
            next: "end"
        },
        {
            token: "string.quoted.double.asp",
            regex: '"',
            next: "start"
        },
        {
            token: "string.quoted.double.asp",
            regex: "'",
            next: "start"
        },
        {
            token: "string.quoted.single.asp",
            regex: "'",
            next: "start"
        },
        {
            defaultToken: "string.quoted.double.asp"
        }
    ]
}

};

oop.inherits(FoxProHighlightRules, TextHighlightRules);

exports.FoxProHighlightRules = FoxProHighlightRules;
});