using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Xrm;
using System.Security;
using Microsoft.Xrm.Sdk.Metadata;

namespace CRUD_withOData
{
    class ExecutionMethods
    {
        /// <summary>
        /// Not recommended - because by default 
        /// </summary>
        /// <param name="_serviceProxy"></param>
        public static void newExecuteMultipleRequest(OrganizationServiceProxy _serviceProxy)
        {
            try
            {
                Console.WriteLine("Check Account & Contact forms and its records for DEMO_RECORD_1");
                Console.WriteLine();
                // Create first account
                OrganizationRequest req1 = new OrganizationRequest();
                req1.RequestName = "Create";
                Guid newAccountId = Guid.NewGuid();
                var account = new Account()
                {
                    Name = "Acme, Inc.",
                    Id = newAccountId
                };
                req1.Parameters.Add("Target", account);

                // Create second account
                OrganizationRequest req2 = new OrganizationRequest();
                account.Name = "Updated name of Acme, Inc.";
                req2.RequestName = "Update";
                req2.Parameters.Add("Target", account);

                // Using Execute Multiple 
                ExecuteTransactionRequest multipleRequest = new ExecuteTransactionRequest();
                multipleRequest.Requests = new OrganizationRequestCollection();
                multipleRequest.Requests.Add(req1);
                multipleRequest.Requests.Add(req2);

                var responseForRecords = (ExecuteTransactionResponse)_serviceProxy.Execute(multipleRequest);
                
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                Console.WriteLine("Transaction rolled back because: {0}", ex.Message);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
