from django.contrib import admin
from django.contrib.auth.admin import UserAdmin
from django.contrib.auth.models import Group

from django.utils.translation import gettext_lazy as _

from .models import *


class UserAdmin(UserAdmin):
    search_fields = (
        "email",
        "lastname",
        "firstname",
    )

    ordering = ("-date_joined",)

    list_display = (
        "email",
        "lastname",
        "firstname",
        "is_active",
        "is_staff",
        "is_superuser",
    )

    list_filter = (
        "is_active",
        "is_staff",
        "is_superuser",
    )

    fieldsets = (
        (
            None,
            {
                "fields": ("password",),
            },
        ),
        (
            _("Персональная информация"),
            {
                "fields": (
                    "email",
                    "lastname",
                    "firstname",
                ),
            },
        ),
        (
            _("Права доступа"),
            {
                "fields": (
                    "is_active",
                    "is_staff",
                    "is_superuser",
                ),
            },
        ),
        (
            _("Важные даты"),
            {
                "fields": (
                    "last_login",
                    "date_joined",
                    "date_of_birth",
                ),
            },
        ),
    )

    add_fieldsets = (
        (
            None,
            {
                "fields": (
                    "password1",
                    "password2",
                ),
            },
        ),
        (
            _("Персональная информация"),
            {
                "fields": (
                    "email",
                    "lastname",
                    "firstname",
                ),
            },
        ),
        (
            _("Права доступа"),
            {
                "fields": (
                    "is_active",
                    "is_staff",
                    "is_superuser",
                ),
            },
        ),
        (
            _("Важные даты"),
            {
                "fields": (
                    "last_login",
                    "date_joined",
                    "date_of_birth",
                ),
            },
        ),
    )


admin.site.unregister(Group)

admin.site.register(User, UserAdmin)
