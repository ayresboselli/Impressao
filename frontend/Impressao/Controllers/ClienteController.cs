using Impressao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Impressao.Controllers
{
    public class ClienteController : Controller
    {
        // GET: ClienteController
        public ActionResult Index()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("ver_clientes"))
            { 
                DataContext context = new();
                List<Cliente> clientes = context.Cliente.ToList();
                for (int i = 0; i < clientes.Count; i++)
                {
                    clientes[i].Pedidos = context.Pedido.Where(c => c.ClienteId == clientes[i].Id).ToList();
                }

                for (int i = 0; i < clientes.Count; i++)
                {
                    //clientes[i].ClienteGrupo = context.ClienteGrupo.Find(clientes[i].ClienteGrupoId);
                    clientes[i].Cidade = context.Cidade.Find(clientes[i].CidadeId);
                }

                if (TempData["success"] != null)
                {
                    ViewBag.Success = TempData["success"];
                    TempData["success"] = null;
                }

                return View(clientes);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult ValidaId(int id)
        {
            DataContext context = new();
            Cliente cliente = context.Cliente.Find(id);

            return Json(new { valid = cliente == null });
        }

        public JsonResult Grupos()
        {
            DataContext context = new();
            List<ClienteGrupo> grupo = context.ClienteGrupo.ToList();
            return Json(grupo);
        }

        // GET: ClienteController/Create
        public ActionResult Create()
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_clientes"))
            {
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ClienteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cliente cliente)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_clientes"))
            {
                try
                {
                    if (cliente.ClienteGrupoId == 0)
                    {
                        ModelState["ClienteGrupoId"].Errors.Add("Selecione um grupo");
                        ModelState["ClienteGrupoId"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                    }
                    if (cliente.CidadeId == 0)
                    {
                        ModelState["CidadeId"].Errors.Add("Selecione uma cidade");
                        ModelState["CidadeId"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                    }
                    if (cliente.FisicaJuridica == 'J')
                    {
                        if (!IsCnpj(cliente.CpfCnpj))
                        {
                            ModelState["CpfCnpj"].Errors.Add("CNPJ inválido");
                            ModelState["CpfCnpj"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                        }
                    }
                    else
                    {
                        if (!IsCpf(cliente.CpfCnpj))
                        {
                            ModelState["CpfCnpj"].Errors.Add("CPF inválido");
                            ModelState["CpfCnpj"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                        }
                    }
                    if (!IsEmail(cliente.Email))
                    {
                        ModelState["Email"].Errors.Add("E-mail inválido");
                        ModelState["Email"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                    }
                    if (!IsCEP(cliente.Cep))
                    {
                        ModelState["Cep"].Errors.Add("CEP inválido");
                        ModelState["Cep"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                    }

                    ModelState.Remove("Id");

                    if (ModelState.IsValid)
                    {
                        DataContext context = new();

                        context.Cliente.Add(cliente);
                        context.Entry(cliente).State = EntityState.Added;
                        context.SaveChanges();

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View();
                    }
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ClienteController/Edit/5
        public ActionResult Edit(int? id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_clientes"))
            {
                DataContext context = new();
                var cliente = context.Cliente.Find(id);

                return View(cliente);
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ClienteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cliente cliente)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_clientes"))
            {
                try
                {
                    if (cliente.ClienteGrupoId == 0)
                    {
                        ModelState["ClienteGrupoId"].Errors.Add("Selecione um grupo");
                        ModelState["ClienteGrupoId"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                    }
                    if (cliente.CidadeId == 0)
                    {
                        ModelState["CidadeId"].Errors.Add("Selecione uma cidade");
                        ModelState["CidadeId"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                    }
                    if (cliente.FisicaJuridica == 'J')
                    {
                        if (!IsCnpj(cliente.CpfCnpj))
                        {
                            ModelState["CpfCnpj"].Errors.Add("CNPJ inválido");
                            ModelState["CpfCnpj"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                        }
                    }
                    else
                    {
                        if (!IsCpf(cliente.CpfCnpj))
                        {
                            ModelState["CpfCnpj"].Errors.Add("CPF inválido");
                            ModelState["CpfCnpj"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                        }
                    }
                    if (!IsEmail(cliente.Email))
                    {
                        ModelState["Email"].Errors.Add("E-mail inválido");
                        ModelState["Email"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                    }
                    if (!IsCEP(cliente.Cep))
                    {
                        ModelState["Cep"].Errors.Add("CEP inválido");
                        ModelState["Cep"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid;
                    }

                    if (ModelState.IsValid)
                    {
                        DataContext context = new();

                        context.Cliente.Update(cliente);
                        context.Entry(cliente).State = EntityState.Modified;
                        context.SaveChanges();

                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        return View();
                    }
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ClienteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            string[] listaFuncoes = HttpContext.Session.GetString("Functions").Split(",");
            if (listaFuncoes.Contains("cad_clientes"))
            {
                try
                {
                    DataContext context = new();
                    Cliente cliente = context.Cliente.Find(id);
                    if (cliente != null)
                    {
                        context.Cliente.Remove(cliente);
                        context.SaveChanges();
                        return Json(new { success = true });
                    }
                    return Json(new { success = false, msg = "Cliente não encontrado" });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, msg = ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, msg = "Acesso negado" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Lista()
        {
            DataContext context = new();
            return Json(context.Cliente.Where(c => c.Ativo).ToList());
        }

        public static bool IsCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;

            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj.Length != 14)
                return false;

            tempCnpj = cnpj.Substring(0, 12);

            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCnpj += digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito += resto.ToString();

            return cnpj.EndsWith(digito);
        }

        public static bool IsCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCpf = tempCpf + digito;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        public static bool IsEmail(string strEmail)
        {
            string strModelo = "^([0-9a-zA-Z]([-.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
            return Regex.IsMatch(strEmail, strModelo);
        }

        public static bool IsCEP(string strCEP)
        {
            string strModelo = "^([0-9]{5}-[0-9]{3})";
            return Regex.IsMatch(strCEP, strModelo);
        }

        private static bool ValidaCliente(Cliente cliente)
        {
            return (
                    cliente.ClienteGrupoId != 0 &&
                    cliente.CidadeId != 0 &&
                    !String.IsNullOrEmpty(cliente.RazaoSocial) &&
                    (IsCnpj(cliente.CpfCnpj) || IsCpf(cliente.CpfCnpj)) &&
                    IsEmail(cliente.Email) &&
                    !String.IsNullOrEmpty(cliente.Telefone) &&
                    !String.IsNullOrEmpty(cliente.Logradouro) &&
                    !String.IsNullOrEmpty(cliente.Bairro) &&
                    IsCEP(cliente.Cep)
                );
            
        }
    }
}
