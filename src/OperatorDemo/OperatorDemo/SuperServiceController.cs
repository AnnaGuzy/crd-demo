using k8s.Models;

using KubeOps.Abstractions.Events;
using KubeOps.Abstractions.Reconciliation;
using KubeOps.Abstractions.Reconciliation.Controller;
using KubeOps.KubernetesClient;

using Newtonsoft.Json;

namespace OperatorDemo
{
    public class SuperServiceController(
        EventPublisher _eventPublisher, 
        ILogger<SuperServiceController> _logger, 
        IKubernetesClient _kubernetesClient) 
        : IEntityController<SuperServiceEntity>
    {
        public async Task<ReconciliationResult<SuperServiceEntity>> ReconcileAsync(SuperServiceEntity entity, CancellationToken cancellationToken)
        {
            var configMap = new V1ConfigMap
            {
                Metadata = new V1ObjectMeta
                {
                    Name = $"{entity.Name()}-config",
                    NamespaceProperty = entity.Namespace()
                },
                Data = new Dictionary<string, string>
                {
                    { "config.json", JsonConvert.SerializeObject(entity.Spec, Formatting.Indented) }
                }
            };
            configMap.AddOwnerReference(new V1OwnerReference
            {
                ApiVersion = entity.ApiVersion,
                Kind = entity.Kind,
                Name = entity.Name(),
                Uid = entity.Uid()
            });
            await _kubernetesClient.SaveAsync(configMap, cancellationToken);

            var deployment = new V1Deployment
            {
                Metadata = new V1ObjectMeta
                {
                    Name = $"{entity.Name()}-deployment",
                    NamespaceProperty = entity.Namespace()
                },
                Spec = new V1DeploymentSpec
                {
                    Replicas = 1,
                    Selector = new V1LabelSelector
                    {
                        MatchLabels = new Dictionary<string, string>
                        {
                            { "app", $"{entity.Name()}-app" }
                        }
                    },
                    Template = new V1PodTemplateSpec
                    {
                        Metadata = new V1ObjectMeta
                        {
                            Labels = new Dictionary<string, string>
                            {
                                { "app", $"{entity.Name()}-app" }
                            }
                        },
                        Spec = new V1PodSpec
                        {
                            Containers = new List<V1Container>
                            {
                                new V1Container
                                {
                                    Name = $"{entity.Name()}-container",
                                    Image = "registry.k8s.io/pause:3.9",
                                    //Command = [ "/bin/bash", "-c", "--" ],
                                    //Args = [ "while true; do sleep 30; done;" ],
                                    VolumeMounts = new List<V1VolumeMount>
                                    {
                                        new V1VolumeMount
                                        {
                                            Name = $"{entity.Name()}-config-volume",
                                            MountPath = "/etc/config"
                                        }
                                    },
                                }
                            },
                            Volumes = new List<V1Volume>
                            {
                                new V1Volume
                                {
                                    Name = $"{entity.Name()}-config-volume",
                                    ConfigMap = new V1ConfigMapVolumeSource
                                    {
                                        Name = $"{entity.Name()}-config",
                                    }
                                }
                            }
                        }
                    }
                }
            };
            deployment.AddOwnerReference(new V1OwnerReference
            {
                ApiVersion = entity.ApiVersion,
                Kind = entity.Kind,
                Name = entity.Name(),
                Uid = entity.Uid()
            });
            await _kubernetesClient.SaveAsync(deployment, cancellationToken);

            await _eventPublisher(
                entity,
                "Reconciled",
                "SuperServiceEntity was successfully reconciled",
                EventType.Normal,
                cancellationToken);

            _logger.LogInformation("Entity {Name} has been reconciled.", entity.Name());

            return ReconciliationResult<SuperServiceEntity>.Success(entity);
        }

        public async Task<ReconciliationResult<SuperServiceEntity>> DeletedAsync(SuperServiceEntity entity, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Entity {Name} has been deleted.", entity.Name());

            // all handled by kubernetes

            await _eventPublisher(
                entity,
                "Deleted",
                "SuperServiceEntity was successfully deleted",
                EventType.Normal,
                cancellationToken);
            return ReconciliationResult<SuperServiceEntity>.Success(entity);
        }
    }
}
