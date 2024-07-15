from rest_framework import serializers
from rest_framework.serializers import ModelSerializer

from .models import *
from authentication.models import *


class CustomDateTimeField(serializers.DateTimeField):
    def to_representation(self, value):
        return value.strftime("%d.%m.%Y %H:%M")


class CustomDateField(serializers.DateField):
    def to_representation(self, value):
        return value.strftime("%d.%m.%Y")


class CustomTimeField(serializers.TimeField):
    def to_representation(self, value):
        return value.strftime("%H:%M")


class UserSerializer(ModelSerializer):
    class Meta:
        model = User
        fields = "__all__"

        depth = 1

class UserRatingSerializer(ModelSerializer):
    class Meta:
        model = User
        fields = ['firstname', 'lastname', 'score']


class AttestationSeriallizer(ModelSerializer):
    class Meta:
        model = Attestation
        fields = "__all__"

        depth = 1
