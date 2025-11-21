using Xunit;
using LibLab4;
using System;
using System.Collections.Generic;
using System.Linq;

public class Lab4Tests
{
    #region Truth Table Generation by Number

    [Fact]
    public void GenerateTruthTableByNumber_WithValidInput_ShouldGenerateCorrectTable()
    {
        // Arrange
        int numVariables = 2;
        int functionNumber = 1;
        var variableNames = new List<string> { "x1", "x2" };

        // Act
        var result = LibLab4.LibLab4.GenerateTruthTableByNumber(numVariables, functionNumber, variableNames);

        // Assert
        Assert.Equal(4, result.Count);
        Assert.Equal(0, result[0].Result);
        Assert.Equal(0, result[1].Result);
        Assert.Equal(0, result[2].Result);
        Assert.Equal(1, result[3].Result);
    }

    [Fact]
    public void GenerateTruthTableByNumber_WithTautology_ShouldGenerateAllOnes()
    {
        // Arrange
        int numVariables = 2;
        int functionNumber = 15;
        var variableNames = new List<string> { "x1", "x2" };

        // Act
        var result = LibLab4.LibLab4.GenerateTruthTableByNumber(numVariables, functionNumber, variableNames);

        // Assert
        Assert.All(result, row => Assert.Equal(1, row.Result));
    }

    [Fact]
    public void GenerateTruthTableByNumber_WithContradiction_ShouldGenerateAllZeros()
    {
        // Arrange
        int numVariables = 2;
        int functionNumber = 0;
        var variableNames = new List<string> { "x1", "x2" };

        // Act
        var result = LibLab4.LibLab4.GenerateTruthTableByNumber(numVariables, functionNumber, variableNames);

        // Assert
        Assert.All(result, row => Assert.Equal(0, row.Result));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(21)]
    public void GenerateTruthTableByNumber_WithInvalidVariableCount_ShouldThrowException(int numVariables)
    {
        // Arrange
        int functionNumber = 1;
        var variableNames = Enumerable.Range(1, numVariables > 0 ? numVariables : 1).Select(i => $"x{i}").ToList();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LibLab4.LibLab4.GenerateTruthTableByNumber(numVariables, functionNumber, variableNames));
    }

    #endregion

    #region Formula Parsing and Evaluation

    [Fact]
    public void GetVariablesFromFormula_WithValidFormula_ShouldExtractVariables()
    {
        // Arrange
        string formula = "(x1 | x2) -> !x3";

        // Act
        var result = LibLab4.LibLab4.GetVariablesFromFormula(formula);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains("x1", result);
        Assert.Contains("x2", result);
        Assert.Contains("x3", result);
    }

    [Fact]
    public void GetVariablesFromFormula_WithEmptyFormula_ShouldReturnEmptyList()
    {
        // Arrange
        string formula = "";

        // Act
        var result = LibLab4.LibLab4.GetVariablesFromFormula(formula);

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("x1 & x2 | x3", new[] { "x1", "x2", "&", "x3", "|" })]
    [InlineData("x1 -> x2 = x3", new[] { "x1", "x2", "->", "x3", "=" })]
    [InlineData("!(x1 | x2)", new[] { "x1", "x2", "|", "!" })]
    [InlineData("x1", new[] { "x1" })]
    public void ConvertToRPN_WithVariousFormulas_ShouldConvertCorrectly(string formula, string[] expectedRpnArray)
    {
        // Arrange
        var expectedRPN = expectedRpnArray.ToList();

        // Act
        var result = LibLab4.LibLab4.ConvertToRPN(formula);

        // Assert
        Assert.Equal(expectedRPN, result);
    }

    [Theory]
    [InlineData("(x1 & x2")]
    [InlineData("x1 & x2)")]
    public void ConvertToRPN_WithMismatchedParentheses_ShouldThrowException(string formula)
    {
        // Act & Assert
        Assert.Throws<Exception>(() => LibLab4.LibLab4.ConvertToRPN(formula));
    }

    [Fact]
    public void ConvertToRPN_WithInvalidCharacter_ShouldThrowException()
    {
        // Arrange
        string formula = "x1 @ x2";

        // Act & Assert
        Assert.Throws<Exception>(() => LibLab4.LibLab4.ConvertToRPN(formula));
    }

    [Fact]
    public void GenerateTruthTableByFormula_WithSimpleFormula_ShouldGenerateCorrectTable()
    {
        // Arrange
        string formula = "!x1 | x2";
        var variables = new List<string> { "x1", "x2" };

        // Act
        var result = LibLab4.LibLab4.GenerateTruthTableByFormula(formula, variables);

        // Assert
        Assert.Equal(4, result.Count);
        Assert.Equal(1, result[0].Result);
        Assert.Equal(1, result[1].Result);
        Assert.Equal(0, result[2].Result);
        Assert.Equal(1, result[3].Result);
    }

    #endregion

    #region DNF and KNF Generation

    [Fact]
    public void GenerateDNFAndKNF_WithTautology_ShouldReturnCorrectForms()
    {
        // Arrange
        var tableData = new List<TruthTableRow>
        {
            CreateTruthTableRow(new List<string>{"x1"}, new List<int>{0}, 1),
            CreateTruthTableRow(new List<string>{"x1"}, new List<int>{1}, 1)
        };
        var variables = new List<string> { "x1" };

        // Act
        var (dnf, knf) = LibLab4.LibLab4.GenerateDNFAndKNF(tableData, variables);

        // Assert
        Assert.Equal("(!x1) | (x1)", dnf);
        Assert.Equal("1", knf);
    }

    [Fact]
    public void GenerateDNFAndKNF_WithSimpleFunction_ShouldGenerateCorrectForms()
    {
        // Arrange
        var tableData = new List<TruthTableRow>
    {
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{0, 0}, 1),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{0, 1}, 1),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{1, 0}, 0),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{1, 1}, 1)
    };
        var variables = new List<string> { "x1", "x2" };
        string expectedDNF = "(!x1 & !x2) | (!x1 & x2) | (x1 & x2)";
        string expectedKNF = "(!x1 | x2)";

        // Act
        var (actualDNF, actualKNF) = LibLab4.LibLab4.GenerateDNFAndKNF(tableData, variables);

        // Assert
        Assert.Equal(expectedDNF, actualDNF);
        Assert.Equal(expectedKNF, actualKNF);
    }

    [Fact]
    public void GenerateDNFAndKNF_WithContradiction_ShouldReturnCorrectForms()
    {
        // Arrange
        var tableData = new List<TruthTableRow>
    {
        CreateTruthTableRow(new List<string>{"x1"}, new List<int>{0}, 0),
        CreateTruthTableRow(new List<string>{"x1"}, new List<int>{1}, 0)
    };
        var variables = new List<string> { "x1" };

        // Act
        var (dnf, knf) = LibLab4.LibLab4.GenerateDNFAndKNF(tableData, variables);

        // Assert
        Assert.Equal("0", dnf);
        Assert.Equal("(x1) & (!x1)", knf);
    }

    #endregion

    #region Function Comparison

    [Fact]
    public void CompareTruthTables_WithEquivalentTables_ShouldReturnTrue()
    {
        // Arrange
        var table1 = new List<TruthTableRow>
        {
            CreateTruthTableRow(new List<string>{"x1"}, new List<int>{0}, 1),
            CreateTruthTableRow(new List<string>{"x1"}, new List<int>{1}, 0)
        };
        var table2 = new List<TruthTableRow>
        {
            CreateTruthTableRow(new List<string>{"x1"}, new List<int>{0}, 1),
            CreateTruthTableRow(new List<string>{"x1"}, new List<int>{1}, 0)
        };

        // Act
        bool areEquivalent = LibLab4.LibLab4.CompareTruthTables(table1, table2, out _);

        // Assert
        Assert.True(areEquivalent);
    }

    [Fact]
    public void CompareTruthTables_WithNonEquivalentTables_ShouldReturnFalseAndCounterexample()
    {
        // Arrange
        var table1 = new List<TruthTableRow> // !x1
        {
            CreateTruthTableRow(new List<string>{"x1"}, new List<int>{0}, 1),
            CreateTruthTableRow(new List<string>{"x1"}, new List<int>{1}, 0)
        };
        var table2 = new List<TruthTableRow> // x1
        {
            CreateTruthTableRow(new List<string>{"x1"}, new List<int>{0}, 0),
            CreateTruthTableRow(new List<string>{"x1"}, new List<int>{1}, 1)
        };

        // Act
        bool areEquivalent = LibLab4.LibLab4.CompareTruthTables(table1, table2, out TruthTableRow counterExample);

        // Assert
        Assert.False(areEquivalent);
        Assert.NotNull(counterExample);
        Assert.Equal(1, counterExample.Result);
    }

    [Fact]
    public void CompareTruthTables_ByNumberAndFormula_Equivalent_ShouldReturnTrue()
    {
        // Arrange
        var variables = new List<string> { "x1", "x2" };
        var tableByNumber = LibLab4.LibLab4.GenerateTruthTableByNumber(2, 7, variables);
        var tableByFormula = LibLab4.LibLab4.GenerateTruthTableByFormula("x1 | x2", variables);

        // Act
        bool areEquivalent = LibLab4.LibLab4.CompareTruthTables(tableByNumber, tableByFormula, out _);

        // Assert
        Assert.True(areEquivalent);
    }

    #endregion

    [Fact]
    public void CompareTruthTables_NonEquivalentFormulas_ShouldReturnFalseAndCounterexample()
    {
        // Arrange
        var variables = new List<string> { "x1", "x2" };
        var table1 = LibLab4.LibLab4.GenerateTruthTableByFormula("x1 -> x2", variables);
        var table2 = LibLab4.LibLab4.GenerateTruthTableByFormula("x2 -> x1", variables);

        // Act
        bool areEquivalent = LibLab4.LibLab4.CompareTruthTables(table1, table2, out TruthTableRow counterExample);

        // Assert
        Assert.False(areEquivalent);
        Assert.NotNull(counterExample);

        Assert.Equal(0, counterExample.GetValue("x1"));
        Assert.Equal(1, counterExample.GetValue("x2"));
    }

    [Fact]
    public void GenerateDNFAndKNF_PartiallyTrueFunction_ShouldGenerateCorrectDNF()
    {
        // Arrange
        var tableData = new List<TruthTableRow>
    {
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{0, 0}, 0),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{0, 1}, 1),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{1, 0}, 1),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{1, 1}, 0)
    };
        var variables = new List<string> { "x1", "x2" };
        string expectedDNF = "(!x1 & x2) | (x1 & !x2)";
        string expectedKNF = "(x1 | x2) & (!x1 | !x2)";

        var (actualDNF, actualKNF) = LibLab4.LibLab4.GenerateDNFAndKNF(tableData, variables);

        Assert.Equal(expectedDNF, actualDNF);
        Assert.Equal(expectedKNF, actualKNF);
    }

    [Fact]
    public void GenerateTruthTableByNumber_InvalidFunctionNumber_ShouldThrowException()
    {
        // Arrange
        int numVariables = 3;
        int functionNumber = 256;
        var variableNames = new List<string> { "x1", "x2", "x3" };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            LibLab4.LibLab4.GenerateTruthTableByNumber(numVariables, functionNumber, variableNames));
    }

    [Theory]
    [InlineData("x1 & x2 | x3", new[] { "x1", "x2", "&", "x3", "|" })]
    [InlineData("x1 | x2 & x3", new[] { "x1", "x2", "x3", "&", "|" })]
    public void ConvertToRPN_ConjunctionOverDisjunction_ShouldRespectPriority(string formula, string[] expectedRpnArray)
    {
        var expectedRPN = expectedRpnArray.ToList();
        var result = LibLab4.LibLab4.ConvertToRPN(formula);
        Assert.Equal(expectedRPN, result);
    }

    [Theory]
    [InlineData("x1 -> x2 | x3", new[] { "x1", "x2", "x3", "|", "->" })]
    [InlineData("x1 = x2 & x3", new[] { "x1", "x2", "x3", "&", "=" })]
    public void ConvertToRPN_ImplicationEquivalencePriority_ShouldRespectPriority(string formula, string[] expectedRpnArray)
    {
        var expectedRPN = expectedRpnArray.ToList();
        var result = LibLab4.LibLab4.ConvertToRPN(formula);
        Assert.Equal(expectedRPN, result);
    }
    [Fact]
    public void ConvertToRPN_NestedParentheses_ShouldConvertCorrectly()
    {
        string formula = "!( (x1 | x2) & x3 )";
        string[] expectedRpnArray = { "x1", "x2", "|", "x3", "&", "!" };
        var expectedRPN = expectedRpnArray.ToList();
        var result = LibLab4.LibLab4.ConvertToRPN(formula);
        Assert.Equal(expectedRPN, result);
    }
    [Fact]
    public void ConvertToRPN_ComplexFormulaWithAllOperators_ShouldConvertCorrectly()
    {
        string formula = "x1 & x2 -> !(x3 = x4)";
        string[] expectedRpnArray = { "x1", "x2", "&", "x3", "x4", "=", "!", "->" };
        var expectedRPN = expectedRpnArray.ToList();
        var result = LibLab4.LibLab4.ConvertToRPN(formula);
        Assert.Equal(expectedRPN, result);
    }
    [Fact]
    public void ConvertToRPN_MultipleNegations_ShouldConvertCorrectly()
    {
        string formula = "!!x1 | !!!x2";
        string[] expectedRpnArray = { "x1", "!", "!", "x2", "!", "!", "!", "|" };
        var expectedRPN = expectedRpnArray.ToList();
        var result = LibLab4.LibLab4.ConvertToRPN(formula);
        Assert.Equal(expectedRPN, result);
    }
    [Fact]
    public void GetVariablesFromFormula_ComplexFormula_ShouldExtractAllVariables()
    {
        string formula = "!(x10 | x2) -> (x3 & x4)";
        var result = LibLab4.LibLab4.GetVariablesFromFormula(formula);
        Assert.Equal(4, result.Count);
        Assert.Equal(new List<string> { "x2", "x3", "x4", "x10" }, result);
    }

    [Fact]
    public void GenerateTruthTableByFormula_ImplicationAndConjunction_ShouldBeCorrect()
    {
        string formula = "x1 -> x2 & x3";
        var variables = new List<string> { "x1", "x2", "x3" };
        var table = LibLab4.LibLab4.GenerateTruthTableByFormula(formula, variables);

        var row = table.First(r => r.GetValue("x1") == 1 && r.GetValue("x2") == 1 && r.GetValue("x3") == 0);
        Assert.Equal(0, row.Result);
    }

    [Fact]
    public void GenerateTruthTableByFormula_Equivalence_ShouldBeCorrect()
    {
        string formula = "x1 = x2";
        var variables = new List<string> { "x1", "x2" };
        var table = LibLab4.LibLab4.GenerateTruthTableByFormula(formula, variables);

        var row1 = table.First(r => r.GetValue("x1") == 1 && r.GetValue("x2") == 0);
        Assert.Equal(0, row1.Result);
        var row2 = table.First(r => r.GetValue("x1") == 1 && r.GetValue("x2") == 1);
        Assert.Equal(1, row2.Result);
    }

    [Fact]
    public void GenerateTruthTableByFormula_Tautology_ShouldBeAllOnes()
    {
        string formula = "!(x1 & x2) = (!x1 | !x2)";
        var variables = new List<string> { "x1", "x2" };
        var table = LibLab4.LibLab4.GenerateTruthTableByFormula(formula, variables);
        Assert.All(table, row => Assert.Equal(1, row.Result));
    }

    [Fact]
    public void GenerateTruthTableByFormula_FourVariables_ShouldGenerateCorrectTable()
    {
        string formula = "(x1 & x2) | (x3 & x4)";
        var variables = new List<string> { "x1", "x2", "x3", "x4" };
        var table = LibLab4.LibLab4.GenerateTruthTableByFormula(formula, variables);
        Assert.Equal(16, table.Count);

        var row = table.First(r => r.GetValue("x1") == 0 && r.GetValue("x2") == 0 && r.GetValue("x3") == 1 && r.GetValue("x4") == 1);
        Assert.Equal(1, row.Result);
    }

    [Fact]
    public void GenerateTruthTableByFormula_Xor_ShouldBeCorrect()
    {
        string formula = "x1 ^ x2";
        var variables = new List<string> { "x1", "x2" };
        var table = LibLab4.LibLab4.GenerateTruthTableByFormula(formula, variables);

        var row = table.First(r => r.GetValue("x1") == 1 && r.GetValue("x2") == 1);
        Assert.Equal(0, row.Result);
    }

    [Fact]
    public void GenerateDNFAndKNF_ThreeVariables_ShouldGenerateCorrectForms()
    {
        var tableData = new List<TruthTableRow>
    {
        CreateTruthTableRow(new List<string>{"x1", "x2", "x3"}, new List<int>{0, 0, 0}, 0),
        CreateTruthTableRow(new List<string>{"x1", "x2", "x3"}, new List<int>{0, 0, 1}, 0),
        CreateTruthTableRow(new List<string>{"x1", "x2", "x3"}, new List<int>{0, 1, 0}, 0),
        CreateTruthTableRow(new List<string>{"x1", "x2", "x3"}, new List<int>{0, 1, 1}, 0),
        CreateTruthTableRow(new List<string>{"x1", "x2", "x3"}, new List<int>{1, 0, 0}, 1),
        CreateTruthTableRow(new List<string>{"x1", "x2", "x3"}, new List<int>{1, 0, 1}, 0),
        CreateTruthTableRow(new List<string>{"x1", "x2", "x3"}, new List<int>{1, 1, 0}, 1),
        CreateTruthTableRow(new List<string>{"x1", "x2", "x3"}, new List<int>{1, 1, 1}, 0)
    };
        var variables = new List<string> { "x1", "x2", "x3" };
        string expectedDNF = "(x1 & !x2 & !x3) | (x1 & x2 & !x3)";
        string expectedKNF = "(x1 | x2 | x3) & (x1 | x2 | !x3) & (x1 | !x2 | x3) & (x1 | !x2 | !x3) & (!x1 | x2 | !x3) & (!x1 | !x2 | !x3)";

        var (actualDNF, actualKNF) = LibLab4.LibLab4.GenerateDNFAndKNF(tableData, variables);

        Assert.Equal(expectedDNF, actualDNF);
        Assert.Equal(expectedKNF, actualKNF);
    }

    [Fact]
    public void GenerateDNFAndKNF_Contradiction_ShouldReturnCorrectForms()
    {
        var tableData = new List<TruthTableRow>
    {
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{0, 0}, 0),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{0, 1}, 0),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{1, 0}, 0),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{1, 1}, 0)
    };
        var variables = new List<string> { "x1", "x2" };

        var (dnf, knf) = LibLab4.LibLab4.GenerateDNFAndKNF(tableData, variables);

        Assert.Equal("0", dnf);
        Assert.Equal("(x1 | x2) & (x1 | !x2) & (!x1 | x2) & (!x1 | !x2)", knf);
    }

    [Fact]
    public void GenerateDNFAndKNF_Tautology_ShouldReturnCorrectForms()
    {
        var tableData = new List<TruthTableRow>
    {
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{0, 0}, 1),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{0, 1}, 1),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{1, 0}, 1),
        CreateTruthTableRow(new List<string>{"x1", "x2"}, new List<int>{1, 1}, 1)
    };
        var variables = new List<string> { "x1", "x2" };

        var (dnf, knf) = LibLab4.LibLab4.GenerateDNFAndKNF(tableData, variables);

        Assert.Equal("(!x1 & !x2) | (!x1 & x2) | (x1 & !x2) | (x1 & x2)", dnf);
        Assert.Equal("1", knf);
    }

    [Fact]
    public void GenerateTruthTableByNumber_MaxVariables_ShouldGenerateCorrectSize()
    {
        int numVariables = 5;
        int functionNumber = 0;
        var variableNames = Enumerable.Range(1, numVariables).Select(i => $"x{i}").ToList();

        var result = LibLab4.LibLab4.GenerateTruthTableByNumber(numVariables, functionNumber, variableNames);
        Assert.Equal(32, result.Count);
        Assert.All(result, row => Assert.Equal(0, row.Result));
    }

    [Fact]
    public void CompareTruthTables_EquivalentComplexFormulas_ShouldReturnTrue()
    {
        var variables = new List<string> { "x1", "x2", "x3" };
        var table1 = LibLab4.LibLab4.GenerateTruthTableByFormula("x1 & (x2 | x3)", variables);
        var table2 = LibLab4.LibLab4.GenerateTruthTableByFormula("(x1 & x2) | (x1 & x3)", variables);

        bool areEquivalent = LibLab4.LibLab4.CompareTruthTables(table1, table2, out _);
        Assert.True(areEquivalent);
    }


    [Fact]
    public void CompareTruthTables_FormulaVsNumber_Equivalent_ShouldReturnTrue()
    {
        var variables = new List<string> { "x1", "x2" };
        var tableByFormula = LibLab4.LibLab4.GenerateTruthTableByFormula("x1 ^ x2", variables);
        var tableByNumber = LibLab4.LibLab4.GenerateTruthTableByNumber(2, 6, variables);

        bool areEquivalent = LibLab4.LibLab4.CompareTruthTables(tableByFormula, tableByNumber, out _);
        Assert.True(areEquivalent);
    }

    [Fact]
    public void CompareTruthTables_FunctionWithItself_ShouldReturnTrue()
    {
        var variables = new List<string> { "x1", "x2", "x3" };
        var table = LibLab4.LibLab4.GenerateTruthTableByFormula("(x1 & !x2) | x3", variables);

        bool areEquivalent = LibLab4.LibLab4.CompareTruthTables(table, table, out _);
        Assert.True(areEquivalent);
    }

    [Fact]
    public void CompareTruthTables_TautologyVsContradiction_ShouldReturnFalse()
    {
        var variables = new List<string> { "x1" };
        var tableTautology = LibLab4.LibLab4.GenerateTruthTableByFormula("x1 | !x1", variables);
        var tableContradiction = LibLab4.LibLab4.GenerateTruthTableByFormula("x1 & !x1", variables);

        bool areEquivalent = LibLab4.LibLab4.CompareTruthTables(tableTautology, tableContradiction, out _);
        Assert.False(areEquivalent);
    }

    [Fact]
    public void GenerateTruthTableByFormula_FormulaWithSpaces_ShouldBeParsedCorrectly()
    {
        string formula = " x1   & x2 |  !x3 ";
        var variables = new List<string> { "x1", "x2", "x3" };
        var tableWithSpaces = LibLab4.LibLab4.GenerateTruthTableByFormula(formula, variables);
        var tableWithoutSpaces = LibLab4.LibLab4.GenerateTruthTableByFormula("x1 & x2 | !x3", variables);

        bool areEquivalent = LibLab4.LibLab4.CompareTruthTables(tableWithSpaces, tableWithoutSpaces, out _);
        Assert.True(areEquivalent);
    }

    private static TruthTableRow CreateTruthTableRow(List<string> variableNames, List<int> values, int result)
    {
        var row = new TruthTableRow { Result = result };
        for (int i = 0; i < variableNames.Count; i++)
        {
            row.SetValue(variableNames[i], values[i]);
        }
        return row;
    }
}