from django.db import models

from django.utils import timezone
from django.utils.translation import gettext_lazy as _

from django.contrib.auth.models import AbstractBaseUser, PermissionsMixin

from .managers import UserManager


class User(AbstractBaseUser, PermissionsMixin):
    email = models.EmailField(verbose_name=_("Адрес электронной почты"), unique=True)

    lastname = models.CharField(verbose_name=_("Фамилия"), max_length=255)
    firstname = models.CharField(verbose_name=_("Имя"), max_length=255)

    is_active = models.BooleanField(
        verbose_name=_("Активный"),
        help_text=_(
            "Отметьте, если пользователь должен считаться активным. Уберите эту отметку вместо удаления учётной записи."
        ),
        default=True,
    )
    is_staff = models.BooleanField(
        verbose_name=_("Статус персонала"),
        help_text=_(
            "Отметьте, если пользователь является персоналом."
        ),
        default=False,
    )
    is_superuser = models.BooleanField(
        verbose_name=_("Статус суперпользователя"),
        help_text=_(
            "Указывает, что пользователь имеет все права."
        ),
        default=False,
    )

    date_of_birth = models.DateField(
        verbose_name=_("Дата рождения"), blank=True, null=True
    )
    date_joined = models.DateTimeField(
        verbose_name=_("Дата регистрации"), default=timezone.now
    )

    score = models.IntegerField(default=0)

    USERNAME_FIELD = "email"
    REQUIRED_FIELDS = []

    objects = UserManager()

    def __str__(self):
        return self.email
