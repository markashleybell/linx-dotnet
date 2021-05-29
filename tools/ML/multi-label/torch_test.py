
import torch
cuda = torch.cuda.is_available()
x = torch.rand(5, 3)
print('CUDA:{}'.format(cuda))
print(x)

