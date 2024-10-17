#region

using Newtonsoft.Json;
using RulesEngine.Models;

#endregion

namespace RegisterMe.Application.FunctionalTests.DataGenerators;

public static class WorkflowGenerator
{
    public static IEnumerable<Workflow> GetWorkflows()
    {
        const string catRegistrationWorkflow = """
                                               
                                                           [
                                                               {
                                                                   "WorkflowName": "MnozstevniSleva",
                                                                   "Rules": [
                                                                       {
                                                                           "RuleName": "MassDiscountForOneAndTwoDays",
                                                                           "ErrorMessage": "One or more adjust rules failed.",
                                                                           "ErrorType": "Error",
                                                                           "RuleExpressionType": "LambdaExpression",
                                                                           "Expression": "input1.SortedByPriceIndex >= 2",
                                                                           "Actions": {
                                                                               "OnSuccess": {
                                                                                   "Name": "OutputExpression",
                                                                                   "Context": {
                                                                                       "Expression": "(input1.numberOfAttendatedDays == 1 ? 600 : 900) - input1.originalPrice"
                                                                                   }
                                                                               },
                                                                               "OnFailure": {
                                                                                 "Name": "OutputExpression",
                                                                                   "Context": {
                                                                                       "Expression": "0"
                                                                                   }
                                                                              }
                                                                           }
                                                                       }                 
                                                                   ]
                                                               },
                                                               {
                                                                   "WorkflowName": "VelkaPujcenaKlec",
                                                                   "RuleName": "Rule1",
                                                                     "Operator": "Or",
                                                                     "ErrorMessage": "One or more adjust rules failed.",
                                                                     "ErrorType": "Error",
                                                                   "Rules": [
                                                                       {
                                                                           "RuleName": "PriceForOneDayDoubleCage",
                                                                           "SuccessEvent": "10",
                                                                           "ErrorMessage": "One or more adjust rules failed.",
                                                                           "ErrorType": "Error",
                                                                           "RuleExpressionType": "LambdaExpression",
                                                                           "Expression": "input1.Cages[10]==1"
                                                                       },
                                                                       {
                                                                           "RuleName": "PriceFOrTwoDaysDoubleCage",
                                                                           "SuccessEvent": "20",
                                                                           "ErrorMessage": "One or more adjust rules failed.",
                                                                           "ErrorType": "Error",
                                                                           "RuleExpressionType": "LambdaExpression",
                                                                           "Expression": "input1.Cages[10]==2"
                                                                       }
                                                                   ]
                                                               },
                                                {
                                                                   "WorkflowName": "VelkaVlastniKlec",
                                                                   "RuleName": "Rule1",
                                                                     "Operator": "Or",
                                                                     "ErrorMessage": "One or more adjust rules failed.",
                                                                     "ErrorType": "Error",
                                                                   "Rules": [
                                                                       {
                                                                           "RuleName": "PriceForOneDayLargeCage",
                                                                           "SuccessEvent": "11",
                                                                           "ErrorMessage": "One or more adjust rules failed.",
                                                                           "ErrorType": "Error",
                                                                           "RuleExpressionType": "LambdaExpression",
                                                                           "Expression": "utils.FindAnyCage(input1.OwnCages, 60, 60, 60, 1, 1) = true"
                                                                       },
                                                                       {
                                                                           "RuleName": "PriceFOrTwoDaysLargeCage",
                                                                           "SuccessEvent": "21",
                                                                           "ErrorMessage": "One or more adjust rules failed.",
                                                                           "ErrorType": "Error",
                                                                           "RuleExpressionType": "LambdaExpression",
                                                                           "Expression": "utils.FindAnyCage(input1.OwnCages, 60, 60, 60, 1, 2) == true"
                                                                       }
                                                                   ]
                                                               }
                                                           ]
                                               """;
        IEnumerable<Workflow> catRegistrationWorkflowworkflow =
            JsonConvert.DeserializeObject<List<Workflow>>(catRegistrationWorkflow) ?? [];


        return catRegistrationWorkflowworkflow;
    }
}
