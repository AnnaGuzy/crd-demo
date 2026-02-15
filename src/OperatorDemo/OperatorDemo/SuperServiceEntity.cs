using k8s.Models;

using KubeOps.Abstractions.Entities;

namespace OperatorDemo
{
    [KubernetesEntity(ApiVersion = "v1alpha1", Group = "demo.holisticon.com", Kind = "SuperService", PluralName = "superservices")]
    public class SuperServiceEntity: CustomKubernetesEntity<SuperServiceSpec>
    {
    }

    public class SuperServiceSpec
    {
        public List<Entry> SuperDictionary { get; set; }
        public int SuperNumber { get; set; }
        public int? OtherNumber { get; set; }
    }

    public class Entry
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
