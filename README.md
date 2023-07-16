# 字符串表达式执行引擎

支持netcore2.0 + 

# NUGET

Install-Package FastExpressionEngine

## Document



```csharp
	var bre = new ExpressionEngine();
	dynamic datas = new ExpandoObject();
	datas.count = 1;
	datas.name = "avqqq";
	var inputs = new dynamic[]
	{
		datas
	};

	var resultList =
		bre.Execute("count < 3 AND name.Contains(\"av\") AND name.StartsWith(\"av\")", inputs);

	var resultListIsSuccess = resultList.IsSuccess;
```

项目参考 https://github.com/microsoft/RulesEngine
表达式编译采用：https://github.com/dadhi/FastExpressionCompiler
缓存采用LRU默认1000size：https://github.com/bitfaster/BitFaster.Caching
