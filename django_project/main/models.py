from django.db import models

from django.utils import timezone
from django.utils.translation import gettext_lazy as _


class AttestationQuestion(models.Model):
    name = models.CharField(max_length=255)
    type = models.CharField(max_length=255)
    values = models.CharField(max_length=255, null=True, blank=True)
    rightValues = models.CharField(max_length=255)

    def __str__(self):
        return self.type


class Attestation(models.Model):
    name = models.CharField(max_length=255)

    attestationQuestions = models.ManyToManyField(
        AttestationQuestion
    )

    def __str__(self):
        return self.name
