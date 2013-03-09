var albumList = [
               { Name: "Lucy Ford", Artist: "Atmosphere", ReleaseYear:2007 },
                { Name: "The Chronic", Artist: "Dr. Dre", ReleaseYear: 1992 },
              { Name: "The Pretty Toney Album", Artist: "GhostFace", ReleaseYear: 2011 },
              { Name: "First Rodeo", Artist: "Honey Honey", ReleaseYear: 2007 },
               { Name: "Speakerboxxxx/The Love Below", Artist: "OutKast", ReleaseYear: 2003 },
              { Name: "By All Means Necessary", Artist: "Boogie Down Productions", ReleaseYear: 1990 },
                { Name: "Headphone Masterpiece", Artist: "Cody Chestnutt", ReleaseYear: 2002 },
                  { Name: "3 ft High and Rising", Artist: "De La Soul", ReleaseYear: 1989 },
                 { Name: "Aquemini", Artist: "OutKast", ReleaseYear: 1998 },     
     { Name: "ATLiens", Artist: "OutKast", ReleaseYear: 1996 },
                { Name: "De La Soul is Dead", Artist: "De La Soul", ReleaseYear: 1991 }
];
test("query where test", function () {

    var eo = new Query(albumList)
                    .where(function (item) { return item.Name == "ATLiens" && item.Artist == "OutKast"; })
                    .execute();

    var res = { Name: "ATLiens", Artist: "OutKast", ReleaseYear: 1996 };
    deepEqual(eo[0], res, "");

});
test("query any test", function () {

    var any = new Query(albumList).any(function (item) { return item.Artist == "OutKast"; });

    ok(any===true, "");
});
test("query not any test", function () {

    var any = new Query(albumList).any(function (item) { return item.Artist == "Public Enemy"; });

    ok(any== false, "Passed!");
});
test("query skip take count test", function () {
    var skipTake = new Query(albumList)
                   .skip(4)
                   .take(3).execute();
    ok(skipTake.length === 3, "");
});
test("query skip take object test", function () {

    var skipTake = new Query(albumList)
                   .skip(4)
                   .take(3).execute();
    var obj = { Name: "Speakerboxxxx/The Love Below", Artist: "OutKast", ReleaseYear: 2003 };
    deepEqual(skipTake[0], obj, "");
});
test("query distinct test", function() {
    var q = new Query(albumList)
                  .distinct(function (item) { return item.Artist; })
                  .execute();
    ok(q.length === 8, "");

});
test("query reverse test", function () {
    var r = new Query(albumList).reverse().execute();
    var obj = { Name: "De La Soul is Dead", Artist: "De La Soul", ReleaseYear: 1991 };
    deepEqual(r[0], obj, "");

});
test("query count test", function () {
    
    var cnt = new Query(albumList).count(function (item) {
        return item.Artist == "OutKast";
    });
    ok(cnt === 3, "");

});
test("query select test", function () {
    var q = new Query(albumList)
                   .where(function (item) { return item.Artist == "OutKast"; })
                   .orderBy(function (item) { return item.ReleaseYear; })
               .select("Name,ReleaseYear")
                   .execute();
    var obj = { Name: "ATLiens", ReleaseYear: 1996 };
    deepEqual(q[0], obj, "");

});
test("odata contains test", function () {
    var q = new Olink('http://odata.netflix.com/Catalog').
                   from('Titles').
                   where("Name.contains('Rose')").
                   execute();


    ok(q.trim() == "http://odata.netflix.com/Catalog/Titles?$filter=substringof('Rose',Name) eq true&$format=json", "");
});
test("odata year function test", function() {
    var q = new Olink('http://odata.netflix.com/Catalog').
        from('Titles').
        where("DateModified.Year() == 2012").
        execute();
    ok(q.trim() === "http://odata.netflix.com/Catalog/Titles?$filter=year(DateModified)  eq 2012&$format=json", "");
});
test("odata toupper  test", function() {
    var q = new Olink('http://odata.netflix.com/Catalog').
        from('Titles').
        where("Name.toUpperCase()=='THE NAME OF THE ROSE'").
        execute();
    ok(q.trim() === "http://odata.netflix.com/Catalog/Titles?$filter=toupper(Name)eq'THE NAME OF THE ROSE'&$format=json", "");
});
test("odata toupper  test", function() {
    var q = new Olink('http://odata.netflix.com/Catalog').
        from('Titles').
        where("Name.toUpperCase()=='THE NAME OF THE ROSE'").
        execute();
    ok(q.trim() === "http://odata.netflix.com/Catalog/Titles?$filter=toupper(Name)eq'THE NAME OF THE ROSE'&$format=json", "");
});
test("odata length test", function() {
    var q = new Olink('http://odata.netflix.com/Catalog').
        from('Titles').
        where("Name.length() > 165").
        execute();
    ok(q.trim() === "http://odata.netflix.com/Catalog/Titles?$filter=length(Name) gt 165&$format=json", "");
});
test("odata and test", function () {
    var q = new Olink('http://odata.netflix.com/Catalog').
        from('Titles').
       where("ReleaseYear == 1986").
       and("AverageRating == 3.6").
       execute();
    
    ok(q.trim() === "http://odata.netflix.com/Catalog/Titles?$filter=ReleaseYear eq 1986 and AverageRating eq 3.6&$format=json", "");
});