using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Xrm;
using System.Security;
using Microsoft.Xrm.Sdk.Metadata;
using System.Runtime.Serialization;
using Microsoft.Xrm.Sdk.Query;

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
                Console.WriteLine("New record in Account entity will be created as 'Acme, Inc.' and then updated to 'Updated name of Acme, Inc.'");
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
                Console.WriteLine("RESULT - Check Account entity's records for Updated name of Acme, Inc.");
                
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


        public static void RetriveMultipleWithQueryExpression(OrganizationServiceProxy _serviceProxy)
        {
            Console.WriteLine("*****************************");
            QueryExpression query = new QueryExpression("account");
            query.ColumnSet.AllColumns = true;
            query.Criteria.AddCondition("name", ConditionOperator.NotNull);
            query.AddOrder("name", OrderType.Descending);
            EntityCollection collection = _serviceProxy.RetrieveMultiple(query);
            Console.WriteLine("Latest count of Account is: " + collection.Entities.Count);
            Console.WriteLine("=============================");
            foreach (Entity entity in collection.Entities)
            {
                Console.WriteLine(entity.Attributes["name"].ToString());
            }
            Console.WriteLine("*****************************");
        }

        public static void RetriveMultipleWithFetchXML(OrganizationServiceProxy _serviceProxy) {
            string fetchxml = "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>" +
                   "<entity name='account'>" +
                     "<attribute name='name'/>" +
                      "<attribute name='accountnumber'/>" +
                     "<order descending='false' attribute='name'/>" +
                     "<filter type='and'>" +
                      //" <condition attribute='name' value='Sample Account Name' operator='eq'/>" +
                      "<condition attribute='name' operator='not-null'/>" +
                      "</filter>" +
                   "</entity>" +
                 "</fetch>";

            EntityCollection entityCollection = _serviceProxy.RetrieveMultiple(new FetchExpression(fetchxml));
            Console.WriteLine("Latest count of Account is: " + entityCollection.Entities.Count);
            Console.WriteLine("=============================");
            foreach (Entity Account in entityCollection.Entities)
            {
                Console.WriteLine(Account.Attributes["name"].ToString());
            }
            Console.WriteLine("*****************************");
        }

        ///<summary>
        ///Sample with Create and Update same entity in SampleCode version
        ///</summary>
        public static void newCreateAndUpdateRequest(OrganizationServiceProxy _serviceProxy)
        {
            ExecuteTransactionRequest requestForRecords = null;
            requestForRecords = new ExecuteTransactionRequest()
            {
                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection(),
                ReturnResponses = true
            };

            // Create several (local, in memory) entities in a collection. 
            EntityCollection input = GetCollectionOfEntitiesToCreate();

            // Add a CreateRequest for each entity to the request collection.
            foreach (var entity in input.Entities)
            {
                CreateRequest createRequest = new CreateRequest { Target = entity };
                requestForRecords.Requests.Add(createRequest);
            }

            // Execute all the requests in the request collection using a single web method call.
            try
            {
                var responseForCreateRecords =
                    (ExecuteTransactionResponse)_serviceProxy.Execute(requestForRecords);

                int i = 0;
                // Display the results returned in the responses.
                foreach (var responseItem in responseForCreateRecords.Responses)
                {
                    if (responseItem != null)
                    {
                        Console.WriteLine("Created " + ((Account)requestForRecords.Requests[i].Parameters["Target"]).Name
                           + " with account id as " + responseItem.Results["id"].ToString());
                    }
                    i++;
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Console.WriteLine("Create request failed for the account{0} and the reason being: {1}",
                    ((ExecuteTransactionFault)(ex.Detail)).FaultedRequestIndex + 1, ex.Detail.Message);
                throw;
            }
        }

        private static EntityCollection GetCollectionOfEntitiesToCreate()
        {
            EntityCollection accountEntities = new EntityCollection();
            for (int i = 0; i <= 4;)
            {
                Guid newAccountId = Guid.NewGuid();
                var account = new Account()
                {
                    Name = "Account ID " + i.ToString(),
                    Id = newAccountId
                };
                accountEntities.Entities.Add(account);
            }

            // Code to update the any of the entity store in 'accountEntities'.

            return new EntityCollection()
            {
                EntityName = Account.EntityLogicalName,
                Entities = {
                    new Account { Name = "ExecuteTransaction Example Account 1" },
                    new Account { Name = "ExecuteTransaction Example Account 2" },
                    new Account { Name = "ExecuteTransaction Example Account 3" },
                    new Account { Name = "ExecuteTransaction Example Account 4" }
                }
            };
        }
    }
}
