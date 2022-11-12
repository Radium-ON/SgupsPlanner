using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SgupsPlanner.Core.Views;
using Prism.Services.Dialogs;

namespace SgupsPlanner.Core.Extensions
{
    public static class DialogServiceExtensions
    {
        /// <summary>
        /// Вызывает модальное окно с кнопкой Ок
        /// </summary>
        /// <param name="dialogService">Объект службы диалога <see cref="IDialogService"/>.</param>
        /// <param name="title">Заголовок окна.</param>
        /// <param name="message">Сообщение для пользователя.</param>
        /// <param name="callBack">Метод, запускаемый после закрытия окна диалога.</param>
        public static void ShowOkDialog(this IDialogService dialogService, string title, string message, Action<IDialogResult> callBack)
        {
            dialogService.ShowDialog(nameof(OkDialog), new DialogParameters { { "Title", $"{title}" }, { "Message", $"{message}" } }, callBack);
        }
        /// <summary>
        /// Вызывает модальное окно с кнопками Ок и Отмена
        /// </summary>
        /// <param name="dialogService">Объект службы диалога <see cref="IDialogService"/>.</param>
        /// <param name="title">Заголовок окна.</param>
        /// <param name="message">Сообщение для пользователя.</param>
        /// <param name="callBack">Метод, запускаемый после закрытия окна диалога.</param>
        public static void ShowOkCancelDialog(this IDialogService dialogService, string title, string message, Action<IDialogResult> callBack)
        {
            dialogService.ShowDialog(nameof(OkCancelDialog), new DialogParameters { { "Title", $"{title}" }, { "Message", $"{message}" } }, callBack);
        }
    }
}
