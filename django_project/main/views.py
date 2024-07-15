import json
import datetime
import pytz

from django.shortcuts import render

from django.utils import timezone

from rest_framework.decorators import api_view, permission_classes
from rest_framework.permissions import IsAuthenticated
from rest_framework.response import Response

from authentication.models import *
from .models import *
from authentication.serializers import *
from .serializers import *


@api_view(["GET"])
def GetRating(request):
    if request.method == "GET":
        queryset = User.objects.all()
        serializer = UserRatingSerializer(queryset, many=True)

        return Response(serializer.data)


@api_view(["GET"])
@permission_classes([IsAuthenticated])
def GetUserData(request):
    if request.method == "GET":
        serializer = UserSerializer(request.user, many=False)

        return Response(serializer.data)


@api_view(["GET"])
def GetAttestations(request):
    if request.method == "GET":
        queryset = Attestation.objects.all()
        serializer = AttestationSeriallizer(queryset, many=True)

        return Response(serializer.data)


@api_view(["GET", "PUT", "DELETE"])
def GetAttestationById(request, id):
    if request.method == "GET":
        queryset = Attestation.objects.get(id=id)
        serializer = AttestationSeriallizer(queryset, many=False)

        return Response(serializer.data)

    if request.method == "PUT":
        queryset = Attestation.objects.get(id=id)

        data = json.loads(request.body)

        # print(data)

        attestation_questions_queryset = []

        for question in data["AttestationQuestions"]:
            attestation_question = AttestationQuestion.objects.filter(id=question["Id"])

            if attestation_question.exists():
                attestation_questions_queryset.append(attestation_question)
            else:
                question_data = {
                    "name": question["Name"],
                    "type": question["Type"],
                    "values": question["Values"],
                    "rightValues": question["RightValues"],
                }
                new_attestation_question = AttestationQuestion(**question_data)
                new_attestation_question.save()
                attestation_questions_queryset.append(new_attestation_question)

        queryset.attestationQuestions.set(attestation_questions_queryset)

        return Response(status=200)

    if request.method == "DELETE":
        queryset = Attestation.objects.get(id=id)
        queryset.delete()

        return Response(status=200)


# POST
@api_view(["POST"])
@permission_classes([IsAuthenticated])
def UserChange(request):
    if request.method == "POST":
        data = json.loads(request.body)

        # print(data)
        # print(request.user)

        user = request.user

        if "FirstName" in data and data["FirstName"]:
            user.firstname = data["FirstName"]
        if "LastName" in data and data["LastName"]:
            user.lastname = data["LastName"]
        if "Email" in data and data["Email"]:
            user.email = data["Email"]
        if "Password" in data and data["Password"]:
            user.set_password(data["Password"])

        user.save()

        return Response(status=200)


@api_view(["POST"])
def CreateAttestation(request):
    if request.method == "POST":
        data = json.loads(request.body)

        id = data["AttestationQuestions"][0]["Id"]

        attestation_questions_queryset = []

        for question in data["AttestationQuestions"]:
            question_data = {
                "name": question["Name"],
                "type": question["Type"],
                "values": question["Values"],
                "rightValues": question["RightValues"],
            }
            attestation_question = AttestationQuestion(**question_data)

            # Save the instance to the database
            attestation_question.save()

            # Add the saved instance to the queryset list
            attestation_questions_queryset.append(attestation_question)

        attestation = Attestation(
            **{
                "name": data["Title"],
            }
        )
        attestation.save()
        attestation.attestationQuestions.set(attestation_questions_queryset)

        return Response(status=200)


@api_view(["POST"])
@permission_classes([IsAuthenticated])
def VAttestationDone(request):
    if request.method == "POST":
        data = json.loads(request.body)

        user = request.user

        user.score += int(data["Score"])
        user.save()

        return Response(status=200)
