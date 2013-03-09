var Expression = function(value) {

    var body = value,
        query = '',
        parse = function() {
            var d = replaceOperator();
            d = rewriteFunction(d);
            d = parseProperty(d);
            query = d;

            return query;
        },
        parseFunction = function() {
          
        },
        replaceOperator = function() {

            var mtc = body.match(/>=|>|<=|<|==|!=/g);
            if (mtc) {
                for (var i = 0; i < mtc.length; i++) {

                    switch (mtc[i].trim()) {
                    case '==':
                        body = body.replace(/==/g, 'eq');
                        break;
                    case '!=':
                        body = body.replace(/!=/g, 'neq');
                        break;
                    case '>=':
                        body = body.replace(/>=/g, 'ge');
                        break;
                    case '>':
                        body = body.replace(/>/g, 'gt');
                        break;
                    case '<=':
                        body = body.replace(/<=/g, 'le');
                        break;
                    case '<':
                        body = body.replace(/</g, 'lt');
                        break;
                    }
                }
            }
            return body;

        },
        rewriteFunction = function(d) {
            var args = d.match(/length|tolowercase|touppercase|year\(\)|month\(\)|day\(\)|contains|startswith|endswith/gi);
            if (args) {
                // var operationStack = new Array();

                for (var i = 0; i < args.length; i++) {
                    var switcher = args[i].toLowerCase();

                    switch (switcher) {
                    case 'length':
                        d = rewriteLength(d);
                        break;
                    case "touppercase":
                    case "tolowercase":
                        d = rewriteCase(d, args[i]);
                        break;
                    case "year()":
                    case "month()":
                    case "day()":
                        d = rewriteDateCompare(d, args[i]);
                        break;
                    case "contains":
                        d = rewriteContains(d);
                        break;
                   /* case "startswith":
                        d = rewriteStartsWith();
                        break;
                    case "endswith":
                        d = rewriteEndsWith();
                        break;*/
                          
                    }
                }
            }
            return d;
        },
        rewriteLength = function(body) {

            var mtc = body.match(/(^|\s)\w+\.length\(\)(?=.|$)/);
            if (mtc) {
                for (var i = 0; i < mtc.length; i++) {
                    if (mtc[i] !== '' && mtc[i] !== ' ') {
                        var position = mtc[i].search(".length()");
                        var sub = mtc[i].substring(0, position);
                        body = body.replace(mtc[i].trim(), "length(" + sub + ")");
                    }

                }

            }
            return body;

        },
        rewriteCase = function(body, dir) {
            var pattern = new RegExp("(^|\\s)\\w+\\." + dir + "\\(\\)(?=.|$)", "i");

            var replacePattern = new RegExp("." + dir + "\(\)", "i");
            var method = dir.toLowerCase() == "touppercase" ? "toupper" : "tolower";
            var mtc = pattern.exec(body);

            if (mtc) {
                for (var i = 0; i < mtc.length; i++) {
                    if (mtc[i] !== '' && mtc[i] !== ' ') {

                        var position = mtc[i].search(replacePattern);

                        var sub = mtc[i].substring(0, position);

                        body = body.replace(mtc[i].trim(), method + "(" + sub + ")");

                    }

                }

            }
            return body;
        },
        rewriteDateCompare = function(body, timeType) {
            var pattern = new RegExp("(^|\\s)\\w+\\." + timeType + "\\(\\)(?=.|$)", "i");

            var replacePattern = new RegExp("." + timeType + "\(\)", "i");
            var subPosition = timeType.search(/\(\)/i);

            var method = timeType.substring(0, subPosition).toLowerCase();
            var mtc = pattern.exec(body);

            if (mtc) {
                for (var i = 0; i < mtc.length; i++) {
                    if (mtc[i] !== '' && mtc[i] !== ' ') {

                        var position = mtc[i].search(replacePattern);

                        var sub = mtc[i].substring(0, position);

                        body = body.replace(mtc[i].trim(), method + "(" + sub + ") ");

                    }

                }

            }
            return body;
        },
        rewriteContains = function (data) {

            var mtc = data.match(/(^|\s)\w+\.contains\(.*\)(?=.|$)/i);

            if (mtc) {
                for (var i = 0; i < mtc.length; i++) {
                    if (mtc[i] !== '' && mtc[i] !== ' ') {
                        var position = mtc[i].search(".contains");
                        
                        var prop = mtc[i].substring(0, position);
                        var start = mtc[i].search("\\(");
                        var end = mtc[i].search("\\)");
         
                        var val = mtc[i].substring(start+1, end);
       

                        body = body.replace(mtc[i].trim(), "substringof(" + val+ ","+prop+") eq true");
                    }

                }

            }
            return body;
        },
        rewriteStartsWith = function() {
            
        },
        rewriteEndsWith = function() {
            
        },
        rewrite = function() {

            if (isfunction) {

            } else {

                var lft = parseProperty(left);

                var op = parseOperator(operator);
                console.log(lft);
                console.log(op);
                console.log(right);
                return lft + ' ' + op + ' ' + right;
            }
        },
        parseOperator = function(op) {
            switch (op) {
            case '==':
                return 'eq';
            case '!=':
                return 'neq';
            }
        },
        parseProperty = function(d) {
            var args = d.match(/\./g);
            if (args) {
                for (var i = 0; i < args.length; i++) {

                    if (isNaN(args[i])) {
                        var replace = args[i].replace(/\./g, '/');
                        d.replace(args[i], replace);
                    }
                }

            }
            return d;
        } ;

    return {
        parse: parse,
        parseFunction:parseFunction,
        rewrite:rewrite
        
    };


}