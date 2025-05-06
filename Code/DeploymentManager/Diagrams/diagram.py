# diagram.py
from diagrams import Diagram, Cluster
from diagrams.aws.compute import EC2
from diagrams.aws.database import RDS
from diagrams.aws.network import ELB
from diagrams.saas.cdn import Cloudflare
from diagrams.azure.compute import *
from diagrams.azure.database import *
from diagrams.azure.general import *
from diagrams.azure.integration import *
from diagrams.azure.security import *
from diagrams.azure.network import *
from diagrams.elastic.saas import *
from diagrams.onprem.vcs import *
from diagrams.onprem.queue import *
from diagrams.onprem.network import *
from diagrams.onprem.monitoring import *
from diagrams.onprem.logging import *
from diagrams.onprem.gitops import *
from diagrams.onprem.database import *
from diagrams.onprem.ci import *
from diagrams.onprem.certificates import *
from diagrams.azure.database import *
from diagrams.elastic.elasticsearch import *

with Diagram("SaaS B2B system", show=False, direction="LR"):

    # declare resources
    webApp = AKS("Blazor")
    api = AKS("API")
    batchProcessor = AKS("Batch processor")
    serviceBus = ServiceBus("Service Bus")
    containerRegisty = ContainerRegistries("Container Registry")
    grafanaCloud = Grafana("Grafana Cloud")
    github = Github("Github")
    cloudflare = Cloudflare("Cloudflare")
    keyVault = KeyVaults("Key Vault")
    postgreSQL = DatabaseForPostgresqlServers("PostgresSQL")
    blobStorage = BlobStorage("Blob Storage")
    elasticSearchCloud = ElasticSearch("Elastic Search Cloud")   

   
   # group resources into clusters

    # with Cluster("API containers"):
    #     apiContainers = [AKS("1"),
    #                     AKS("2"),
    #                     AKS("3")]        

    # with Cluster("Web App containers"):
    #     webAppContainers = [AKS("1"),
    #                         AKS("2"),
    #                         AKS("3")]        

    # with Cluster("Batch processor containers"):
    #     batchProcessorContainers = [AKS("1"),
    #                                 AKS("2"),
    #                                 AKS("3")]
        

    # make connections between resources

    # cloudflare >> webApp >> webAppContainers >> api >> apiContainers >> serviceBus >> batchProcessor >> batchProcessorContainers >> blobStorage

    # cloudflare >> webApp >> webAppContainers >> api >> apiContainers >> postgreSQL >> serviceBus >> batchProcessorContainers >> blobStorage

    cloudflare >> webApp >> api >> serviceBus >> batchProcessor >> blobStorage
    
    api >> postgreSQL >> serviceBus >> batchProcessor    
    api >> blobStorage
    batchProcessor >> blobStorage
    
    webApp >> grafanaCloud
    api >> grafanaCloud
    batchProcessor >> grafanaCloud

    api >> elasticSearchCloud


    # webApp >> keyVault
    # api >> keyVault
    # batchProcessor >> keyVault
    
    



    # ELB("lb") >> EC2("web") >> RDS("userdb")

    