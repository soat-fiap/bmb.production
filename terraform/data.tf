##############################
# EKS CLUSTER
##############################

data "aws_eks_cluster" "techchallenge_cluster" {
  name = var.eks_cluster_name
}