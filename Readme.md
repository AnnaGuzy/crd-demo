# CRD Demo

## Resources

<https://kubernetes.io/docs/tasks/extend-kubernetes/custom-resources/custom-resource-definitions/>
<https://dotnet.github.io/dotnet-operator-sdk/docs/operator/>

## Useful commands

```powershell
# set alias to kubectl
Set-Alias -Name k -Value kubectl

#select your current namespace once connected to cluster
k config set-context --current --namespace=crd-demo

#apply file with crd or resource instance definition
k apply -f [...]\crd-demo\infra\crds\demo.holisticon.com\super-service.yaml
```
