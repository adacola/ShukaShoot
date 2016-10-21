namespace Adacola.ShukaShoot.Test

open FsUnit
open NUnit.Framework
open Adacola.ShukaShoot.ShukaPolice

[<TestFixture>]
module Police =
    // trueのパターン
    [<Test>]
    let ``isFavoriteTextで"しゅかしゅー"に対してtrueを返すこと`` () =
        "しゅかしゅー" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"しゅかしゅ～"に対してtrueを返すこと`` () =
        "しゅかしゅ～" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"シュカシュー"に対してtrueを返すこと`` () =
        "シュカシュー" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"しゆかしゆー"に対してtrueを返すこと`` () =
        "しゆかしゆー" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"シユカシユー"に対してtrueを返すこと`` () =
        "シユカシユー" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"しゅかしゅう"に対してtrueを返すこと`` () =
        "しゅかしゅう" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"しゅかしゅぅ"に対してtrueを返すこと`` () =
        "しゅかしゅぅ" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"シュカシュウ"に対してtrueを返すこと`` () =
        "シュカシュウ" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"シュカシュゥ"に対してtrueを返すこと`` () =
        "シュカシュゥ" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"しゅかぴ"に対してtrueを返すこと`` () =
        "しゅかぴ" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"シュカピ"に対してtrueを返すこと`` () =
        "シュカピ" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextでしゅかしゅーを含む場合にtrueを返すこと`` () =
        "あんちゃんりきゃこすわわありしゃしゅかしゅーあいきゃんきんぐあいにゃふりりん" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"しゅかしゅー"にスペースが混入してtrueを返すこと`` () =
        " し  ゅ   か    し     ゅ      ー       " |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"しゅかしゅー"に改行が混入してtrueを返すこと`` () =
        "\nし\rゅ\r\nか\n\nし\r\rゅ\r\n\r\nー\n\n\n" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"しゅかしゅー"の縦書きでtrueを返すこと`` () =
        "し\nゅ\nか\nし\nゅ\n l" |> isFavoriteText |> should equal true

    [<Test>]
    let ``isFavoriteTextで"朱夏"に対してtrueを返すこと`` () =
        "朱夏" |> isFavoriteText |> should equal true

    // falseのパターン
    [<Test>]
    let ``isFavoriteTextで"ゅかしゅー"に対してfalseを返すこと`` () =
        "ゅかしゅー" |> isFavoriteText |> should equal false

    [<Test>]
    let ``isFavoriteTextで"しかしゅー"に対してfalseを返すこと`` () =
        "しかしゅー" |> isFavoriteText |> should equal false

    [<Test>]
    let ``isFavoriteTextで"しゅしゅー"に対してfalseを返すこと`` () =
        "しゅしゅー" |> isFavoriteText |> should equal false

    [<Test>]
    let ``isFavoriteTextで"しゅかゅー"に対してfalseを返すこと`` () =
        "しゅかゅー" |> isFavoriteText |> should equal false

    [<Test>]
    let ``isFavoriteTextで"しゅかしゅ"に対してfalseを返すこと`` () =
        "しゅかしゅ" |> isFavoriteText |> should equal false

    [<Test>]
    let ``isFavoriteTextで"#朱夏"に対してfalseを返すこと`` () =
        "#朱夏" |> isFavoriteText |> should equal false

    [<Test>]
    let ``isFavoriteTextで"#朱夏取扱説明書"に対してfalseを返すこと`` () =
        "#朱夏取扱説明書" |> isFavoriteText |> should equal false

    [<Test>]
    let ``isFavoriteTextで"しゅかしゅー警察"に対してfalseを返すこと`` () =
        "しゅかしゅー警察" |> isFavoriteText |> should equal false

    [<Test>]
    let ``isFavoriteTextでしゅかしゅーへのリプに対してfalseを返すこと`` () =
        "@Saito_Shuka しゅかしゅー" |> isFavoriteText |> should equal false
        " @Saito_Shuka しゅかしゅー" |> isFavoriteText |> should equal false
        "しゅかしゅー @Saito_Shuka" |> isFavoriteText |> should equal false
