from django.shortcuts import render

import json
from .models import *

from rest_framework.decorators import api_view, permission_classes
from rest_framework.response import Response

from .serializers import CustomTokenObtainPairSerializer
from rest_framework_simplejwt.views import TokenObtainPairView

# POST
@api_view(["POST"])
# @permission_classes([IsAuthenticated])
def CreateUser(request):
    if request.method == "POST":
        data = json.loads(request.body)

        user = User(
            email=data['email'],
        )
        user.set_password(data['password'])
        user.save()

        return Response(status=200)
    
class CustomTokenObtainPairView(TokenObtainPairView):
    serializer_class = CustomTokenObtainPairSerializer