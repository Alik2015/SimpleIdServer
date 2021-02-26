﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.6.0.0
//      SpecFlow Generator Version:3.6.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SimpleIdServer.OpenBankingApi.Host.Acceptance.Tests.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.6.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class AccountsFeature : object, Xunit.IClassFixture<AccountsFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "Accounts.feature"
#line hidden
        
        public AccountsFeature(AccountsFeature.FixtureData fixtureData, SimpleIdServer_OpenBankingApi_Host_Acceptance_Tests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Accounts", "\tCheck /accounts endpoint", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Get accounts (Basic)")]
        [Xunit.TraitAttribute("FeatureTitle", "Accounts")]
        [Xunit.TraitAttribute("Description", "Get accounts (Basic)")]
        public virtual void GetAccountsBasic()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get accounts (Basic)", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 4
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table4.AddRow(new string[] {
                            "token_endpoint_auth_method",
                            "tls_client_auth"});
                table4.AddRow(new string[] {
                            "response_types",
                            "[token,code,id_token]"});
                table4.AddRow(new string[] {
                            "grant_types",
                            "[client_credentials,authorization_code,implicit]"});
                table4.AddRow(new string[] {
                            "scope",
                            "accounts"});
                table4.AddRow(new string[] {
                            "redirect_uris",
                            "[https://localhost:8080/callback]"});
                table4.AddRow(new string[] {
                            "tls_client_auth_san_dns",
                            "mtlsClient"});
                table4.AddRow(new string[] {
                            "id_token_signed_response_alg",
                            "PS256"});
                table4.AddRow(new string[] {
                            "token_signed_response_alg",
                            "PS256"});
#line 5
 testRunner.When("execute HTTP POST JSON request \'https://localhost:8080/register\'", ((string)(null)), table4, "When ");
#line hidden
#line 16
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 17
 testRunner.And("extract parameter \'client_id\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table5.AddRow(new string[] {
                            "X-Testing-ClientCert",
                            "mtlsClient.crt"});
                table5.AddRow(new string[] {
                            "client_id",
                            "$client_id$"});
                table5.AddRow(new string[] {
                            "scope",
                            "accounts"});
                table5.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
#line 19
 testRunner.And("execute HTTP POST request \'https://localhost:8080/mtls/token\'", ((string)(null)), table5, "And ");
#line hidden
#line 26
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 27
 testRunner.And("extract parameter \'access_token\' from JSON body into \'accessToken\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table6.AddRow(new string[] {
                            "Authorization",
                            "Bearer $accessToken$"});
                table6.AddRow(new string[] {
                            "x-fapi-interaction-id",
                            "guid"});
                table6.AddRow(new string[] {
                            "X-Testing-ClientCert",
                            "mtlsClient.crt"});
                table6.AddRow(new string[] {
                            "data",
                            "{ \"permissions\" : [ \"ReadAccountsBasic\" ] }"});
#line 29
 testRunner.And("execute HTTP POST JSON request \'https://localhost:8080/v3.1/account-access-consen" +
                        "ts\'", ((string)(null)), table6, "And ");
#line hidden
#line 36
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 37
 testRunner.And("extract parameter \'Data.ConsentId\' from JSON body into \'consentId\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 39
 testRunner.And("\'administrator\' confirm consent \'$consentId$\' for accounts \'22289\', with scopes \'" +
                        "accounts\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table7.AddRow(new string[] {
                            "X-Testing-ClientCert",
                            "mtlsClient.crt"});
                table7.AddRow(new string[] {
                            "response_type",
                            "id_token code"});
                table7.AddRow(new string[] {
                            "client_id",
                            "$client_id$"});
                table7.AddRow(new string[] {
                            "state",
                            "state"});
                table7.AddRow(new string[] {
                            "response_mode",
                            "query"});
                table7.AddRow(new string[] {
                            "scope",
                            "accounts"});
                table7.AddRow(new string[] {
                            "redirect_uri",
                            "https://localhost:8080/callback"});
                table7.AddRow(new string[] {
                            "nonce",
                            "nonce"});
                table7.AddRow(new string[] {
                            "state",
                            "MTkCNSYlem"});
                table7.AddRow(new string[] {
                            "claims",
                            "{ id_token: { openbanking_intent_id : { value: \"$consentId$\", essential: true } }" +
                                " }"});
#line 41
 testRunner.And("execute HTTP GET request \'https://localhost:8080/authorization\'", ((string)(null)), table7, "And ");
#line hidden
#line 54
 testRunner.And("extract \'code\' from callback", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table8.AddRow(new string[] {
                            "X-Testing-ClientCert",
                            "mtlsClient.crt"});
                table8.AddRow(new string[] {
                            "client_id",
                            "$client_id$"});
                table8.AddRow(new string[] {
                            "scope",
                            "accounts"});
                table8.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
                table8.AddRow(new string[] {
                            "code",
                            "$code$"});
                table8.AddRow(new string[] {
                            "redirect_uri",
                            "https://localhost:8080/callback"});
#line 56
 testRunner.And("execute HTTP POST request \'https://localhost:8080/mtls/token\'", ((string)(null)), table8, "And ");
#line hidden
#line 65
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 66
 testRunner.And("extract parameter \'access_token\' from JSON body into \'accessToken\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table9.AddRow(new string[] {
                            "Authorization",
                            "Bearer $accessToken$"});
                table9.AddRow(new string[] {
                            "X-Testing-ClientCert",
                            "mtlsClient.crt"});
#line 68
 testRunner.And("execute HTTP GET request \'https://localhost:8080/v3.1/accounts\'", ((string)(null)), table9, "And ");
#line hidden
#line 73
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 75
 testRunner.Then("HTTP status code equals to \'200\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 76
 testRunner.Then("JSON \'Data.Account[0].AccountId\'=\'22289\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 77
 testRunner.Then("JSON \'Data.Account[0].AccountSubType\'=\'CurrentAccount\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 78
 testRunner.Then("JSON doesn\'t exist \'Data.Account[0].Accounts[0].Identification\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 79
 testRunner.Then("JSON doesn\'t exist \'Data.Account[0].Accounts[0].SecondaryIdentification\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Get accounts (Detail)")]
        [Xunit.TraitAttribute("FeatureTitle", "Accounts")]
        [Xunit.TraitAttribute("Description", "Get accounts (Detail)")]
        public virtual void GetAccountsDetail()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get accounts (Detail)", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 81
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table10.AddRow(new string[] {
                            "token_endpoint_auth_method",
                            "tls_client_auth"});
                table10.AddRow(new string[] {
                            "response_types",
                            "[token,code,id_token]"});
                table10.AddRow(new string[] {
                            "grant_types",
                            "[client_credentials,authorization_code,implicit]"});
                table10.AddRow(new string[] {
                            "scope",
                            "accounts"});
                table10.AddRow(new string[] {
                            "redirect_uris",
                            "[https://localhost:8080/callback]"});
                table10.AddRow(new string[] {
                            "tls_client_auth_san_dns",
                            "mtlsClient"});
                table10.AddRow(new string[] {
                            "id_token_signed_response_alg",
                            "PS256"});
                table10.AddRow(new string[] {
                            "token_signed_response_alg",
                            "PS256"});
#line 82
 testRunner.When("execute HTTP POST JSON request \'https://localhost:8080/register\'", ((string)(null)), table10, "When ");
#line hidden
#line 93
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 94
 testRunner.And("extract parameter \'client_id\' from JSON body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table11.AddRow(new string[] {
                            "X-Testing-ClientCert",
                            "mtlsClient.crt"});
                table11.AddRow(new string[] {
                            "client_id",
                            "$client_id$"});
                table11.AddRow(new string[] {
                            "scope",
                            "accounts"});
                table11.AddRow(new string[] {
                            "grant_type",
                            "client_credentials"});
#line 96
 testRunner.And("execute HTTP POST request \'https://localhost:8080/mtls/token\'", ((string)(null)), table11, "And ");
#line hidden
#line 103
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 104
 testRunner.And("extract parameter \'access_token\' from JSON body into \'accessToken\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table12 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table12.AddRow(new string[] {
                            "Authorization",
                            "Bearer $accessToken$"});
                table12.AddRow(new string[] {
                            "x-fapi-interaction-id",
                            "guid"});
                table12.AddRow(new string[] {
                            "X-Testing-ClientCert",
                            "mtlsClient.crt"});
                table12.AddRow(new string[] {
                            "data",
                            "{ \"permissions\" : [ \"ReadAccountsDetail\" ] }"});
#line 106
 testRunner.And("execute HTTP POST JSON request \'https://localhost:8080/v3.1/account-access-consen" +
                        "ts\'", ((string)(null)), table12, "And ");
#line hidden
#line 113
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 114
 testRunner.And("extract parameter \'Data.ConsentId\' from JSON body into \'consentId\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 116
 testRunner.And("\'administrator\' confirm consent \'$consentId$\' for accounts \'22289\', with scopes \'" +
                        "accounts\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table13 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table13.AddRow(new string[] {
                            "X-Testing-ClientCert",
                            "mtlsClient.crt"});
                table13.AddRow(new string[] {
                            "response_type",
                            "id_token code"});
                table13.AddRow(new string[] {
                            "client_id",
                            "$client_id$"});
                table13.AddRow(new string[] {
                            "state",
                            "state"});
                table13.AddRow(new string[] {
                            "response_mode",
                            "query"});
                table13.AddRow(new string[] {
                            "scope",
                            "accounts"});
                table13.AddRow(new string[] {
                            "redirect_uri",
                            "https://localhost:8080/callback"});
                table13.AddRow(new string[] {
                            "nonce",
                            "nonce"});
                table13.AddRow(new string[] {
                            "state",
                            "MTkCNSYlem"});
                table13.AddRow(new string[] {
                            "claims",
                            "{ id_token: { openbanking_intent_id : { value: \"$consentId$\", essential: true } }" +
                                " }"});
#line 118
 testRunner.And("execute HTTP GET request \'https://localhost:8080/authorization\'", ((string)(null)), table13, "And ");
#line hidden
#line 131
 testRunner.And("extract \'code\' from callback", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table14 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table14.AddRow(new string[] {
                            "X-Testing-ClientCert",
                            "mtlsClient.crt"});
                table14.AddRow(new string[] {
                            "client_id",
                            "$client_id$"});
                table14.AddRow(new string[] {
                            "scope",
                            "accounts"});
                table14.AddRow(new string[] {
                            "grant_type",
                            "authorization_code"});
                table14.AddRow(new string[] {
                            "code",
                            "$code$"});
                table14.AddRow(new string[] {
                            "redirect_uri",
                            "https://localhost:8080/callback"});
#line 133
 testRunner.And("execute HTTP POST request \'https://localhost:8080/mtls/token\'", ((string)(null)), table14, "And ");
#line hidden
#line 142
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 143
 testRunner.And("extract parameter \'access_token\' from JSON body into \'accessToken\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table15 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table15.AddRow(new string[] {
                            "Authorization",
                            "Bearer $accessToken$"});
                table15.AddRow(new string[] {
                            "X-Testing-ClientCert",
                            "mtlsClient.crt"});
#line 145
 testRunner.And("execute HTTP GET request \'https://localhost:8080/v3.1/accounts\'", ((string)(null)), table15, "And ");
#line hidden
#line 150
 testRunner.And("extract JSON from body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 152
 testRunner.Then("HTTP status code equals to \'200\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 153
 testRunner.Then("JSON \'Data.Account[0].AccountId\'=\'22289\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 154
 testRunner.Then("JSON \'Data.Account[0].AccountSubType\'=\'CurrentAccount\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 155
 testRunner.Then("JSON \'Data.Account[0].Accounts[0].Identification\'=\'80200110203345\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 156
 testRunner.Then("JSON \'Data.Account[0].Accounts[0].SecondaryIdentification\'=\'00021\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.6.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                AccountsFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                AccountsFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
