from django.contrib import admin
from django.urls import path, include

from django.conf import settings
from django.conf.urls.static import static

from . import views

from authentication.views import *


urlpatterns = [
    # GET
    path("rating/", views.GetRating),
    path("attestations/", views.GetAttestations),
    path("attestations/<int:id>/", views.GetAttestationById),
    # POST
    path("get-user/", views.GetUserData),
    path("user/change/", views.UserChange),
    path("create/attestation/", views.CreateAttestation),
    path("create/user/", CreateUser),
    path("attestation-done/", views.VAttestationDone),
]
