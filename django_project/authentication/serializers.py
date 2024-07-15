from rest_framework import serializers
from rest_framework.serializers import ModelSerializer

from rest_framework_simplejwt.serializers import TokenObtainPairSerializer

from .models import *


class CustomDateTimeField(serializers.DateTimeField):
    def to_representation(self, value):
        return value.strftime("%d.%m.%Y %H:%M")


class CustomDateField(serializers.DateField):
    def to_representation(self, value):
        return value.strftime("%d.%m.%Y")


class CustomTimeField(serializers.TimeField):
    def to_representation(self, value):
        return value.strftime("%H:%M")


class CustomTokenObtainPairSerializer(TokenObtainPairSerializer):
    def validate(self, attrs):
        data = super().validate(attrs)
        data['is_superuser'] = self.user.is_superuser
        return data