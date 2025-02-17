﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SimpleIdServer.IdServer.Host.Acceptance.Tests.Features.GrantTypes
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class AlternativeScenariosFeature : object, Xunit.IClassFixture<AlternativeScenariosFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "AlternativeScenarios.feature"
#line hidden
        
        public AlternativeScenariosFeature(AlternativeScenariosFeature.FixtureData fixtureData, SimpleIdServer_IdServer_Host_Acceptance_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/GrantTypes", "AlternativeScenarios", "\tExecute alternative scenarios", ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public void TestInitialize()
        {
        }
        
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="User-agent is redirected to the login page when elapsed time > authentication tim" +
            "e + default client max age")]
        [Xunit.TraitAttribute("FeatureTitle", "AlternativeScenarios")]
        [Xunit.TraitAttribute("Description", "User-agent is redirected to the login page when elapsed time > authentication tim" +
            "e + default client max age")]
        public void User_AgentIsRedirectedToTheLoginPageWhenElapsedTimeAuthenticationTimeDefaultClientMaxAge()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("User-agent is redirected to the login page when elapsed time > authentication tim" +
                    "e + default client max age", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 4
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 5
 testRunner.Given("authenticate a user and add \'-10\' seconds to the authentication time", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table231 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table231.AddRow(new string[] {
                            "response_type",
                            "id_token"});
                table231.AddRow(new string[] {
                            "client_id",
                            "thirtyFourClient"});
                table231.AddRow(new string[] {
                            "state",
                            "state"});
                table231.AddRow(new string[] {
                            "response_mode",
                            "query"});
                table231.AddRow(new string[] {
                            "scope",
                            "openid email role"});
                table231.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
                table231.AddRow(new string[] {
                            "nonce",
                            "nonce"});
                table231.AddRow(new string[] {
                            "display",
                            "popup"});
#line 6
 testRunner.When("execute HTTP GET request \'http://localhost/authorization\'", ((string)(null)), table231, "When ");
#line hidden
#line 17
 testRunner.Then("redirection url contains \'http://localhost/pwd/Authenticate\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="User-agent is redirected to the login page when elapsed time > authentication tim" +
            "e + max age")]
        [Xunit.TraitAttribute("FeatureTitle", "AlternativeScenarios")]
        [Xunit.TraitAttribute("Description", "User-agent is redirected to the login page when elapsed time > authentication tim" +
            "e + max age")]
        public void User_AgentIsRedirectedToTheLoginPageWhenElapsedTimeAuthenticationTimeMaxAge()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("User-agent is redirected to the login page when elapsed time > authentication tim" +
                    "e + max age", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 19
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 20
 testRunner.Given("authenticate a user and add \'-10\' seconds to the authentication time", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table232 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table232.AddRow(new string[] {
                            "response_type",
                            "id_token"});
                table232.AddRow(new string[] {
                            "client_id",
                            "thirtyFiveClient"});
                table232.AddRow(new string[] {
                            "state",
                            "state"});
                table232.AddRow(new string[] {
                            "response_mode",
                            "query"});
                table232.AddRow(new string[] {
                            "scope",
                            "openid email role"});
                table232.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
                table232.AddRow(new string[] {
                            "nonce",
                            "nonce"});
                table232.AddRow(new string[] {
                            "display",
                            "popup"});
                table232.AddRow(new string[] {
                            "max_age",
                            "2"});
#line 21
 testRunner.When("execute HTTP GET request \'http://localhost/authorization\'", ((string)(null)), table232, "When ");
#line hidden
#line 33
 testRunner.Then("redirection url contains \'http://localhost/pwd/Authenticate\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="auth_time claim is returned in the identity token when it is marked as essential " +
            "claim")]
        [Xunit.TraitAttribute("FeatureTitle", "AlternativeScenarios")]
        [Xunit.TraitAttribute("Description", "auth_time claim is returned in the identity token when it is marked as essential " +
            "claim")]
        public void Auth_TimeClaimIsReturnedInTheIdentityTokenWhenItIsMarkedAsEssentialClaim()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("auth_time claim is returned in the identity token when it is marked as essential " +
                    "claim", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 35
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 36
 testRunner.Given("authenticate a user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table233 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table233.AddRow(new string[] {
                            "response_type",
                            "id_token"});
                table233.AddRow(new string[] {
                            "client_id",
                            "thirtyFiveClient"});
                table233.AddRow(new string[] {
                            "state",
                            "state"});
                table233.AddRow(new string[] {
                            "response_mode",
                            "query"});
                table233.AddRow(new string[] {
                            "scope",
                            "openid email role"});
                table233.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
                table233.AddRow(new string[] {
                            "nonce",
                            "nonce"});
                table233.AddRow(new string[] {
                            "display",
                            "popup"});
                table233.AddRow(new string[] {
                            "claims",
                            "{ \"id_token\": { \"auth_time\": { \"essential\" : true } } }"});
#line 37
 testRunner.When("execute HTTP GET request \'http://localhost/authorization\'", ((string)(null)), table233, "When ");
#line hidden
#line 49
 testRunner.And("extract parameter \'id_token\' from redirect url", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 50
 testRunner.And("extract payload from JWT \'$id_token$\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 52
 testRunner.Then("JWT contains \'auth_time\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="amr and acr are returned in the identity token when acr_values is passed")]
        [Xunit.TraitAttribute("FeatureTitle", "AlternativeScenarios")]
        [Xunit.TraitAttribute("Description", "amr and acr are returned in the identity token when acr_values is passed")]
        public void AmrAndAcrAreReturnedInTheIdentityTokenWhenAcr_ValuesIsPassed()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("amr and acr are returned in the identity token when acr_values is passed", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 54
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 55
 testRunner.Given("authenticate a user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table234 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table234.AddRow(new string[] {
                            "response_type",
                            "id_token"});
                table234.AddRow(new string[] {
                            "client_id",
                            "thirtyFiveClient"});
                table234.AddRow(new string[] {
                            "state",
                            "state"});
                table234.AddRow(new string[] {
                            "response_mode",
                            "query"});
                table234.AddRow(new string[] {
                            "scope",
                            "openid email role"});
                table234.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
                table234.AddRow(new string[] {
                            "nonce",
                            "nonce"});
                table234.AddRow(new string[] {
                            "display",
                            "popup"});
                table234.AddRow(new string[] {
                            "acr_values",
                            "sid-load-01"});
#line 56
 testRunner.When("execute HTTP GET request \'http://localhost/authorization\'", ((string)(null)), table234, "When ");
#line hidden
#line 68
 testRunner.And("extract parameter \'id_token\' from redirect url", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 69
 testRunner.And("extract payload from JWT \'$id_token$\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 71
 testRunner.Then("JWT contains \'amr\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 72
 testRunner.Then("JWT contains \'acr\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 73
 testRunner.Then("JWT has \'amr\'=\'pwd\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 74
 testRunner.Then("JWT has \'acr\'=\'sid-load-01\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="when using offline_scope the client can access to the userinfo even if the end-us" +
            "er is not authenticated")]
        [Xunit.TraitAttribute("FeatureTitle", "AlternativeScenarios")]
        [Xunit.TraitAttribute("Description", "when using offline_scope the client can access to the userinfo even if the end-us" +
            "er is not authenticated")]
        public void WhenUsingOffline_ScopeTheClientCanAccessToTheUserinfoEvenIfTheEnd_UserIsNotAuthenticated()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("when using offline_scope the client can access to the userinfo even if the end-us" +
                    "er is not authenticated", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 76
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 77
 testRunner.Given("authenticate a user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table235 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table235.AddRow(new string[] {
                            "response_type",
                            "code"});
                table235.AddRow(new string[] {
                            "client_id",
                            "thirtySixClient"});
                table235.AddRow(new string[] {
                            "state",
                            "state"});
                table235.AddRow(new string[] {
                            "response_mode",
                            "query"});
                table235.AddRow(new string[] {
                            "scope",
                            "openid email role offline_access"});
                table235.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
                table235.AddRow(new string[] {
                            "nonce",
                            "nonce"});
                table235.AddRow(new string[] {
                            "display",
                            "popup"});
#line 78
 testRunner.When("execute HTTP GET request \'http://localhost/authorization\'", ((string)(null)), table235, "When ");
#line hidden
#line 88
 testRunner.And("extract parameter \'code\' from redirect url", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table236 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table236.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
                table236.AddRow(new string[] {
                            "code",
                            "$code$"});
                table236.AddRow(new string[] {
                            "client_id",
                            "thirtySixClient"});
                table236.AddRow(new string[] {
                            "client_secret",
                            "password"});
                table236.AddRow(new string[] {
                            "redirect_uri",
                            "http://localhost:8080"});
#line 90
 testRunner.And("execute HTTP POST request \'http://localhost/token\'", ((string)(null)), table236, "And ");
#line hidden
#line 98
 testRunner.And("disconnect the user", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 99
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 100
 testRunner.And("extract parameter \'refresh_token\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 101
 testRunner.And("extract parameter \'access_token\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table237 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table237.AddRow(new string[] {
                            "Authorization",
                            "Bearer $access_token$"});
#line 103
 testRunner.And("execute HTTP GET request \'http://localhost/userinfo\'", ((string)(null)), table237, "And ");
#line hidden
#line 107
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 109
 testRunner.Then("HTTP status code equals to \'200\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 110
 testRunner.Then("JSON \'sub\'=\'user\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 111
 testRunner.Then("JSON \'$.role[0]\'=\'role1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 112
 testRunner.Then("JSON \'$.role[1]\'=\'role2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 113
 testRunner.Then("JSON \'email\'=\'email@outlook.fr\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                AlternativeScenariosFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                AlternativeScenariosFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
