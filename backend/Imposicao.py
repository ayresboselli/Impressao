import psycopg2.extras
import os
import shutil
import barcode
import img2pdf

from barcode.writer import ImageWriter
from PIL import Image, ImageDraw, ImageFont
from dotenv import load_dotenv
from multiprocessing import Process, Manager


debug = False
path_root = os.path.dirname(os.path.realpath(__file__))

dotenv_path = path_root+'/.env'
load_dotenv(dotenv_path)

font_path = 'C:\\Program Files\\Piovelli\\backend\\sans-serif.ttf'

Image.LOAD_TRUNCATED_IMAGES = True
dpm = 11.811023622047244 # densidade por milimetro
font = ImageFont.truetype(font_path, 32)
arquivos = []
path_tmp = ''

vCPU = os.cpu_count()

barras_tmp = os.path.join("C:\\Piovelli\\temp","barras")
os.makedirs(barras_tmp, exist_ok=True)


def MargemCorte(img, supDir, infDir, supEsq, infEsq):
    draw = ImageDraw.Draw(img)
    
    try: draw.line([(supDir[0]-(5*dpm), supDir[1]), (supDir[0]-(2*dpm), supDir[1])], fill="black", width=0) 
    except: print('supDir', supDir)
    try: draw.line([(supDir[0], supDir[1]-(5*dpm)), (supDir[0], supDir[1]-(2*dpm))], fill="black", width=0) 
    except: print('supDir', supDir)
    try: draw.line([(supDir[0]+(2*dpm), supDir[1]), (supDir[0]+(5*dpm), supDir[1])], fill="black", width=0) 
    except: print('supDir', supDir)
    try: draw.line([(supDir[0], supDir[1]+(2*dpm)), (supDir[0], supDir[1]+(5*dpm))], fill="black", width=0) 
    except: print('supDir', supDir)

    try: draw.line([(infDir[0]-(5*dpm), infDir[1]), (infDir[0]-(2*dpm), infDir[1])], fill="black", width=0) 
    except: print('infDir', infDir)
    try: draw.line([(infDir[0], infDir[1]-(5*dpm)), (infDir[0], infDir[1]-(2*dpm))], fill="black", width=0) 
    except: print('infDir', infDir)
    try: draw.line([(infDir[0]+(2*dpm), infDir[1]), (infDir[0]+(5*dpm), infDir[1])], fill="black", width=0) 
    except: print('infDir', infDir)
    try: draw.line([(infDir[0], infDir[1]+(2*dpm)), (infDir[0], infDir[1]+(5*dpm))], fill="black", width=0) 
    except: print('infDir', infDir)

    try: draw.line([(supEsq[0]-(5*dpm), supEsq[1]), (supEsq[0]-(2*dpm), supEsq[1])], fill="black", width=0) 
    except: print('supEsq', supEsq)
    try: draw.line([(supEsq[0], supEsq[1]-(5*dpm)), (supEsq[0], supEsq[1]-(2*dpm))], fill="black", width=0) 
    except: print('supEsq', supEsq)
    try: draw.line([(supEsq[0]+(2*dpm), supEsq[1]), (supEsq[0]+(5*dpm), supEsq[1])], fill="black", width=0) 
    except: print('supEsq', supEsq)
    try: draw.line([(supEsq[0], supEsq[1]+(2*dpm)), (supEsq[0], supEsq[1]+(5*dpm))], fill="black", width=0) 
    except: print('supEsq', supEsq)

    try: draw.line([(infEsq[0]-(5*dpm), infEsq[1]), (infEsq[0]-(2*dpm), infEsq[1])], fill="black", width=0) 
    except: print('infEsq', infEsq)
    try: draw.line([(infEsq[0], infEsq[1]-(5*dpm)), (infEsq[0], infEsq[1]-(2*dpm))], fill="black", width=0) 
    except: print('infEsq', infEsq)
    try: draw.line([(infEsq[0]+(2*dpm), infEsq[1]), (infEsq[0]+(5*dpm), infEsq[1])], fill="black", width=0) 
    except: print('infEsq', infEsq)
    try: draw.line([(infEsq[0], infEsq[1]+(2*dpm)), (infEsq[0], infEsq[1]+(5*dpm))], fill="black", width=0) 
    except: print('infEsq', infEsq)
    
    return img


def AddInfo(midea, inf):
    global path_root
    global font_path
    global barras_tmp

    try:
        if inf['Tipo'] == 'texto':
            draw = ImageDraw.Draw(midea)

            #cria imagem de texto
            textsize = draw.textbbox((0,0),inf['Texto'], font)
            txt_width = textsize[2] - textsize[0] + 10
            txt_height = textsize[3] - textsize[1] + 10

            img_inf = Image.new('RGB', (txt_width, txt_height), (255, 255, 255))
            draw = ImageDraw.Draw(img_inf)
            draw.text((5, 0), inf['Texto'], (0,0,0), font=font)

        elif inf['Tipo'] == 'barras':
            options = {
                'module_width':0.4,
                'module_height':8,
                'text_distance':2,
                'font_path':font_path,
                'font_size':4
            }

            filename = barras_tmp + '/'+str(inf['Texto'])
            codeClass = barcode.get_barcode_class('code128')
            codigo_barra = codeClass(str(inf['Texto']).zfill(3), writer=ImageWriter())
            
            if not os.path.exists(filename+'.png'):
                filename = codigo_barra.save(filename, options = options)
            else: 
                filename += '.png'
            
            img_inf = Image.open(filename)



        # rotaciona imagem de texto
        if int(inf['Orientacao']) == 90:
            img_inf = img_inf.transpose(Image.ROTATE_90)
        elif int(inf['Orientacao']) == 180:
            img_inf = img_inf.transpose(Image.ROTATE_180)
        elif int(inf['Orientacao']) == 270:
            img_inf = img_inf.transpose(Image.ROTATE_270)

        # salva texto na imagem
        inf_w = int((inf['PosX']*dpm)-(img_inf.size[0]/2))
        inf_h = int((inf['PosY']*dpm)-(img_inf.size[1]/2))
        midea.paste(img_inf, (inf_w, inf_h))
        img_inf = None

    except Exception as e: 
        if debug:
            print(e)
    finally:
        return midea


def DuplexFV(item, nomeArq, produto, infos, cnt, pedido, album):
    global dpm
    global path_tmp
    global arquivos
    
    try:
        img = Image.open(item)
        midea = Image.new('RGB', (int(produto['LarguraMidia']*dpm), int(produto['AlturaMidia']*dpm)), (255, 255, 255))

        if cnt % 2 == 0:
            # frente
            (larg, alt, pag) = (int(produto['DeslocamentoFrenteX']*dpm), int(produto['DeslocamentoFrenteY']*dpm), 0)
        else:
            # verso
            (larg, alt, pag) = (int(produto['DeslocamentoVersoX']*dpm), int(produto['DeslocamentoVersoY']*dpm), 1)

        midea.paste(img, (larg, alt))
        
        supDir = (larg, alt)
        infDir = (larg, alt+img.size[1])
        supEsq = (larg+img.size[0], alt)
        infEsq = (larg+img.size[0], alt+img.size[1])
        
        midea = MargemCorte(midea, supDir, infDir, supEsq, infEsq)

        # add info to page
        strct = nomeArq.split('/')[-1].split('.')
        del strct[-1]
        nome_arquivo = '.'.join(strct)
        
        for inf in infos:
            if inf['Pagina'] == pag:
                i = {
                    'Pagina': inf['Pagina'],
                    'Tipo': inf['Tipo'],
                    'Texto': inf['Texto']
                        .replace("{nome_arquivo}", nome_arquivo)
                        .replace("{seq_foto}", str(cnt+1))
                        .replace("{pedido}", str(pedido['PedidoId']))
                        .replace("{op}", str(pedido['Id']))
                        .replace("{id_album}", str(album['Id']))
                        .replace("{nome_album}", album['NomeArquivo'])
                        .replace("{nome_pdf}", album['NomeArquivo']+'.pdf'),
                    'PosX': inf['PosX'],
                    'PosY': inf['PosY'],
                    'Orientacao': inf['Orientacao']
                    }
                
                midea = AddInfo(midea, i)

        
        
        filename = str(cnt).zfill(3)+'.'+item.split('/')[-1].split('.')[-1]

        if produto['Giro180'] == 1 and cnt % 2 != 0: #verso
            midea = midea.transpose(Image.ROTATE_180)

        path_filename = path_tmp + '/' + filename
        midea.save(path_filename, 'JPEG', dpi=(300, 300))
        
        i = 0
        while i < len(arquivos):
            if arquivos[i]['NomeArquivo'] == nomeArq:
                arquivos[i]['PathFilename'] = path_filename
            i+=1

    except Exception as e: 
        if debug:
            print(e)
    
    finally:
        img = None
        midea = None


def DuplexPV(item, nomeArq, produto, infos, cnt, pedido, album):
    global dpm
    global path_tmp
    global arquivos

    try:
        img = Image.open(item)
        midea_f = Image.new('RGB', (int(produto['LarguraMidia']*dpm), int(produto['AlturaMidia']*dpm)), (255, 255, 255))
        midea_v = Image.new('RGB', (int(produto['LarguraMidia']*dpm), int(produto['AlturaMidia']*dpm)), (255, 255, 255))

        midea_f.paste(img, (int(produto['DeslocamentoFrenteX']*dpm), int(produto['DeslocamentoFrenteY']*dpm)))

        supDir = ((int(produto['DeslocamentoFrenteX']*dpm)), int(produto['DeslocamentoFrenteY']*dpm))
        infDir = ((int(produto['DeslocamentoFrenteX']*dpm)), int(produto['DeslocamentoFrenteY']*dpm)+img.size[1])
        supEsq = ((int(produto['DeslocamentoFrenteX']*dpm))+img.size[0], int(produto['DeslocamentoFrenteY']*dpm))
        infEsq = ((int(produto['DeslocamentoFrenteX']*dpm))+img.size[0], int(produto['DeslocamentoFrenteY']*dpm)+img.size[1])
        
        midea_f = MargemCorte(midea_f, supDir, infDir, supEsq, infEsq)
        
        # add info to page
        strct = nomeArq.split('/')[-1].split('.')
        del strct[-1]
        nome_arquivo = '.'.join(strct)
        
        for inf in infos:
            if inf['Pagina'] == 0:
                i = {
                    'Pagina': inf['Pagina'],
                    'Tipo': 'texto',
                    'Texto': inf['Texto']
                        .replace("{nome_arquivo}", nome_arquivo)
                        .replace("{seq_foto}", str(cnt+1))
                        .replace("{pedido}", str(pedido['PedidoId']))
                        .replace("{op}", str(pedido['Id']))
                        .replace("{id_album}", str(album['Id']))
                        .replace("{nome_album}", album['NomeArquivo'])
                        .replace("{nome_pdf}", album['NomeArquivo']+'.pdf'),
                    'PosX': inf['PosX'],
                    'PosY': inf['PosY'],
                    'Orientacao': inf['Orientacao']
                    }
                
                midea_f = AddInfo(midea_f, i)
            elif inf['Pagina'] == 1:
                i = {
                    'Pagina': inf['Pagina'],
                    'Tipo': 'texto',
                    'Texto': inf['Texto'].replace("{nome_arquivo}", nome_arquivo).replace("{seq_foto}", str(cnt+1)),
                    'PosX': inf['PosX'],
                    'PosY': inf['PosY'],
                    'Orientacao': inf['Orientacao']
                    }
                
                midea_v = AddInfo(midea_v, i)

        
        

        filename = str(cnt).zfill(3)+'.'+item.split('/')[-1].split('.')[-1]
        fnst = filename.split('.')
        filename_f = str(fnst[0])+'_f.'+fnst[1]
        filename_v = str(fnst[0])+'_v.'+fnst[1]

        path_filename = path_tmp + '/' + filename
        
        midea_f.save(path_tmp + '/' + filename_f, 'JPEG', dpi=(300, 300))
        midea_v.save(path_tmp + '/' + filename_v, 'JPEG', dpi=(300, 300))

        i = 0
        while i < len(arquivos):
            if arquivos[i]['NomeArquivo'] == nomeArq:
                arquivos[i]['PathFilename'] = path_filename
            i+=1
        
    except Exception as e: 
        if debug:
            print(e)
            
        if os.path.exists(path_tmp + '/' + filename_f):
            os.remove(path_tmp + '/' + filename_f)
        if os.path.exists(path_tmp + '/' + filename_v):
            os.remove(path_tmp + '/' + filename_v)

    finally:
        img = None
        midea_f = None
        midea_v = None


def Simplex(item, nomeArq, produto, infos, cnt, pedido, album):
    global dpm
    global path_tmp
    global arquivos
    
    try:
        img = Image.open(item)
        midea = Image.new('RGB', (int(produto['LarguraMidia']*dpm), int(produto['AlturaMidia']*dpm)), (255, 255, 255))
        
        midea.paste(img, (int(produto['DeslocamentoFrenteX']*dpm), int(produto['DeslocamentoFrenteY']*dpm)))
        
        supDir = ((int(produto['DeslocamentoFrenteX']*dpm)), int(produto['DeslocamentoFrenteY']*dpm))
        infDir = ((int(produto['DeslocamentoFrenteX']*dpm)), int(produto['DeslocamentoFrenteY']*dpm)+img.size[1])
        supEsq = ((int(produto['DeslocamentoFrenteX']*dpm))+img.size[0], int(produto['DeslocamentoFrenteY']*dpm))
        infEsq = ((int(produto['DeslocamentoFrenteX']*dpm))+img.size[0], int(produto['DeslocamentoFrenteY']*dpm)+img.size[1])
        
        midea = MargemCorte(midea, supDir, infDir, supEsq, infEsq)

        # add info to page
        strct = nomeArq.split('/')[-1].split('.')
        del strct[-1]
        nome_arquivo = '.'.join(strct)

        for inf in infos:
            if inf['Pagina'] == 0:
                i = {
                    'Pagina': inf['Pagina'],
                    'Tipo': 'texto',
                    'Texto': inf['Texto']
                        .replace("{nome_arquivo}", nome_arquivo)
                        .replace("{seq_foto}", str(cnt+1))
                        .replace("{pedido}", str(pedido['PedidoId']))
                        .replace("{op}", str(pedido['Id']))
                        .replace("{id_album}", str(album['Id']))
                        .replace("{nome_album}", album['NomeArquivo'])
                        .replace("{nome_pdf}", album['NomeArquivo']+'.pdf'),
                    'PosX': inf['PosX'],
                    'PosY': inf['PosY'],
                    'Orientacao': inf['Orientacao']
                }
                
                midea = AddInfo(midea, i)

        
        filename = str(cnt).zfill(3)+'.'+item.split('/')[-1].split('.')[-1]
        path_filename = path_tmp + '/' + filename
        midea.save(path_filename, 'JPEG', dpi=(300, 300))

        i = 0
        while i < len(arquivos):
            if arquivos[i]['NomeArquivo'] == nomeArq:
                arquivos[i]['PathFilename'] = path_filename
            i+=1
        
    except Exception as e: 
        if debug:
            print(e)

    finally:
        img = None
        midea = None


def ProcessaAlbum(row, album, informacoes, config, produto, nome_arqivo):
    global connection
    global cursor
    global path_tmp
    global arquivos

    try:
        sql = 'SELECT * FROM public."PedidoItemArquivo" WHERE "PedidoItemId" = '+str(row['Id'])+' AND "Album" = \''+str(album['Album'])+'\''
        cursor.execute(sql)
        alb = cursor.fetchone()
        if alb == None:
            sql = 'INSERT INTO public."PedidoItemArquivoAlbum"("PedidoItemId", "Album", "NomeArquivo", "DataCadastro", "DataProcessamento") '
            sql += 'VALUES ('+str(row['Id'])+', '+str(album['Album'])+', \''+nome_arqivo+'\', now(), now());'
            cursor.execute(sql)
            connection.commit()

            sql = 'SELECT * FROM public."PedidoItemArquivo" WHERE "PedidoItemId" = '+str(row['Id'])+' AND "Album" = \''+str(album['Album'])+'\''
            cursor.execute(sql)
            alb = cursor.fetchone()

        path_tmp = os.path.join("C:\\Piovelli\\temp",str(row['Id']))
        # if os.path.exists(path_tmp):
        #     shutil.rmtree(path_tmp)
        
        os.makedirs(path_tmp, exist_ok=True)
        
        sql = 'SELECT * FROM public."PedidoItemArquivo" WHERE "PedidoItemId" = '+str(row['Id'])+' AND "Album" = \''+str(album['Album'])+'\' ORDER BY "NomeArquivo"'
        cursor.execute(sql)
        arquivos = cursor.fetchall()
        
        if len(arquivos) > 0:
            cnt = 0
            for arq in arquivos:

                infos = []
                for inf in informacoes:
                    infos.append(inf)

                path = config['PathFotos']+str(row['PedidoId'])+'/'+str(row['Id'])+'/'+str(album['Album'])+'/'+arq['Path']
                if produto['DisposicaoImpressao'] == 'D':
                    if produto['DisposicaoImagem'] == 'D':
                        DuplexFV(path, arq['NomeArquivo'], produto, infos, cnt, row, alb)
                    else:
                        DuplexPV(path, arq['NomeArquivo'], produto, infos, cnt, row, alb)
                else:
                    Simplex(path, arq['NomeArquivo'], produto, infos, cnt, row, alb)

                cnt+=1
            
            lista_saida = []

            qtd = 0
            if (produto['DisposicaoImpressao'] == 'D' and produto['DisposicaoImagem'] == 'D') or produto['DisposicaoImpressao'] != 'D':
                for l in arquivos:
                    lista_saida.append(l['PathFilename'])
                qtd = len(lista_saida)
            else:
                for l in arquivos:
                    fnst = l['PathFilename'].split('.')
                    lista_saida.append(str(fnst[0])+'_f.'+fnst[1])
                    lista_saida.append(str(fnst[0])+'_v.'+fnst[1])
                qtd = int(len(lista_saida)/2)
                

            if len(arquivos) <= qtd:
                pdfFile = config['PathPdf']+nome_arqivo+'.pdf'
                with open(pdfFile, "wb") as f:
                    f.write(img2pdf.convert(lista_saida))
                    f.close()

            os.chmod(pdfFile, 0o777)
        
        return True
    except:
        os.remove(config['PathPdf']+nome_arqivo+'.pdf')
        return False
    finally:
        #shutil.rmtree(path_tmp)
        None

        


def Start():
    global connection
    global cursor

    connection = psycopg2.connect(
        host="localhost",
        user="postgres",
        password="1234",
        database="impressao",
    )
    cursor = connection.cursor(cursor_factory=psycopg2.extras.RealDictCursor)

    sql = 'SELECT * FROM public."Config"'
    cursor.execute(sql)
    config = cursor.fetchone()
    os.makedirs(config['PathPdf'], exist_ok=True)

    sql = 'SELECT * FROM public."PedidoItem" WHERE "DataAprovacao" IS NOT NULL AND "DataImposicao" IS NULL'
    cursor.execute(sql)
    result = cursor.fetchall()

    for row in result:
        try:
            sql = 'SELECT * FROM public."Produto" WHERE "Id" = '+str(row['ProdutoId'])
            cursor.execute(sql)
            produto = cursor.fetchone()

            sql = 'SELECT * FROM public."ProdutoInformacao" WHERE "ProdutoId" = '+str(produto['Id'])
            cursor.execute(sql)
            informacoes = cursor.fetchall()

            
            sql = 'SELECT "Album" FROM public."PedidoItemArquivo" WHERE "PedidoItemId" = '+str(row['Id'])+' GROUP BY "Album" ORDER BY "Album"'
            cursor.execute(sql)
            albuns = cursor.fetchall()
            
            finalizar = True
            for album in albuns:
                nome_arquivo = str(row['PedidoId'])+'_'+str(row['Id'])+'_'+album['Album']
                if not os.path.exists(config['PathPdf']+nome_arquivo+'.pdf'):
                    if not ProcessaAlbum(row, album, informacoes, config, produto, nome_arquivo):
                        finalizar = False

            if finalizar:
                sql = 'UPDATE public."PedidoItem" SET "DataImposicao" = now() WHERE "Id" = '+str(row["Id"])
                cursor.execute(sql)
                connection.commit()
        except:
            None
