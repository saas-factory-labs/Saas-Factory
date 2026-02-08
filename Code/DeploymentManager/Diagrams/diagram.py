# diagram.py
from diagrams import Diagram, Cluster
from diagrams.aws.compute import EC2
from diagrams.aws.database import RDS
from diagrams.aws.network import ELB
from diagrams.saas.cdn import Cloudflare
from diagrams.azure.compute import AKS
from diagrams.azure.database import DatabaseForPostgresqlServers
from diagrams.azure.integration import ServiceBus
from diagrams.azure.security import KeyVaults
from diagrams.azure.storage import BlobStorage
from diagrams.azure.devops import ContainerRegistries
from diagrams.onprem.vcs import Github
from diagrams.onprem.monitoring import Grafana
from diagrams.elastic.elasticsearch import ElasticSearch

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

    with Cluster("API containers"):
        apiContainers = [AKS("1"),
                        AKS("2"),
                        AKS("3")]        

    with Cluster("Web App containers"):
        webAppContainers = [AKS("1"),
                            AKS("2"),
                            AKS("3")]        

    with Cluster("Batch processor containers"):
        batchProcessorContainers = [AKS("1"),
                                    AKS("2"),
                                    AKS("3")]
        

    # make connections between resources

    cloudflare >> webApp >> webAppContainers >> api >> apiContainers >> serviceBus >> batchProcessor >> batchProcessorContainers >> blobStorage

    cloudflare >> webApp >> webAppContainers >> api >> apiContainers >> postgreSQL >> serviceBus >> batchProcessorContainers >> blobStorage

    cloudflare >> webApp >> api >> serviceBus >> batchProcessor >> blobStorage
    
    api >> postgreSQL >> serviceBus >> batchProcessor    
    api >> blobStorage
    batchProcessor >> blobStorage
    
    webApp >> grafanaCloud
    api >> grafanaCloud
    batchProcessor >> grafanaCloud

    api >> elasticSearchCloud


    webApp >> keyVault
    api >> keyVault
    batchProcessor >> keyVault
    
    



    ELB("lb") >> EC2("web") >> RDS("userdb")

    